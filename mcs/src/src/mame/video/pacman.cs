// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using ListBytesPointer = mame.ListPointer<System.Byte>;
using offs_t = System.UInt32;
using tilemap_memory_index = System.UInt32;
using u8 = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    public partial class pacman_state : driver_device
    {
        /*************************************************************************

            Namco Pac Man

        **************************************************************************

            This file is used by the Pac Man, Pengo & Jr Pac Man drivers.

            Pengo & Pac Man are almost identical, the only differences being the
            extra gfx bank in Pengo, and the need to compensate for an hardware
            sprite positioning "bug" in Pac Man.

            Jr Pac Man has the same sprite hardware as Pac Man, the extra bank
            from Pengo and a scrolling playfield at the expense of one color per row
            for the playfield so it can fit in the same amount of ram.

        **************************************************************************/



        /***************************************************************************

          Convert the color PROMs into a more useable format.


          Pac Man has a 32x8 palette PROM and a 256x4 color lookup table PROM.

          Pengo has a 32x8 palette PROM and a 1024x4 color lookup table PROM.

          The palette PROM is connected to the RGB output this way:

          bit 7 -- 220 ohm resistor  -- BLUE
                -- 470 ohm resistor  -- BLUE
                -- 220 ohm resistor  -- GREEN
                -- 470 ohm resistor  -- GREEN
                -- 1  kohm resistor  -- GREEN
                -- 220 ohm resistor  -- RED
                -- 470 ohm resistor  -- RED
          bit 0 -- 1  kohm resistor  -- RED


          Jr. Pac Man has two 256x4 palette PROMs (the three msb of the address are
          grounded, so the effective colors are only 32) and one 256x4 color lookup
          table PROM.

          The palette PROMs are connected to the RGB output this way:

          bit 3 -- 220 ohm resistor  -- BLUE
                -- 470 ohm resistor  -- BLUE
                -- 220 ohm resistor  -- GREEN
          bit 0 -- 470 ohm resistor  -- GREEN

          bit 3 -- 1  kohm resistor  -- GREEN
                -- 220 ohm resistor  -- RED
                -- 470 ohm resistor  -- RED
          bit 0 -- 1  kohm resistor  -- RED

        ***************************************************************************/

        static readonly int [] palette_init_pacman_resistances3 = new int[3] { 1000, 470, 220 };
        static readonly int [] palette_init_pacman_resistances2 = new int[2] { 470, 220 };

        //PALETTE_INIT_MEMBER(pacman_state,pacman)
        public void palette_init_pacman(palette_device palette)
        {
            ListBytesPointer color_prom = new ListBytesPointer(memregion("proms").base_());  //const uint8_t *color_prom = memregion("proms")->base();
            //static const int resistances[3] = { 1000, 470, 220 };
            double [] rweights = new double[3];
            double [] gweights = new double[3];
            double [] bweights = new double[2];
            int i;

            /* compute the color output resistor weights */
            resnet_global.compute_resistor_weights(0, 255, -1.0,
                    3, palette_init_pacman_resistances3, out rweights, 0, 0,
                    3, palette_init_pacman_resistances3, out gweights, 0, 0,
                    2, palette_init_pacman_resistances2, out bweights, 0, 0);

            /* create a lookup table for the palette */
            for (i = 0; i < 32; i++)
            {
                int bit0;
                int bit1;
                int bit2;
                int r;
                int g;
                int b;

                /* red component */
                //bit0 = (color_prom[i] >> 0) & 0x01;
                //bit1 = (color_prom[i] >> 1) & 0x01;
                //bit2 = (color_prom[i] >> 2) & 0x01;
                bit0 = (color_prom[i] >> 0) & 0x01;
                bit1 = (color_prom[i] >> 1) & 0x01;
                bit2 = (color_prom[i] >> 2) & 0x01;
                r = resnet_global.combine_3_weights(rweights, bit0, bit1, bit2);

                /* green component */
                bit0 = (color_prom[i] >> 3) & 0x01;
                bit1 = (color_prom[i] >> 4) & 0x01;
                bit2 = (color_prom[i] >> 5) & 0x01;
                g = resnet_global.combine_3_weights(gweights, bit0, bit1, bit2);

                /* blue component */
                bit0 = (color_prom[i] >> 6) & 0x01;
                bit1 = (color_prom[i] >> 7) & 0x01;
                b = resnet_global.combine_2_weights(bweights, bit0, bit1);

                palette.palette_interface().set_indirect_color(i, new rgb_t((byte)r, (byte)g, (byte)b));
            }

            /* color_prom now points to the beginning of the lookup table */
            color_prom += 32;

            /* allocate the colortable */
            for (i = 0; i < 64 * 4; i++)
            {
                byte ctabentry = (byte)(color_prom[i] & 0x0f);// color_prom[i] & 0x0f;

                /* first palette bank */
                palette.palette_interface().set_pen_indirect((UInt32)i, ctabentry);

                /* second palette bank */
                palette.palette_interface().set_pen_indirect((UInt32)(i + 64 * 4), (UInt16)(0x10 + ctabentry));
            }
        }


        //TILEMAP_MAPPER_MEMBER(pacman_state::pacman_scan_rows)
        tilemap_memory_index pacman_scan_rows(UInt32 col, UInt32 row, UInt32 num_cols, UInt32 num_rows)
        {
            UInt32 offs;

            row += 2;
            col -= 2;
            if ((col & 0x20) > 0)
                offs = row + ((col & 0x1f) << 5);
            else
                offs = col + (row << 5);

            return offs;
        }

        //TILE_GET_INFO_MEMBER(pacman_state::pacman_get_tile_info)
        void pacman_get_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            int code = m_videoram[tile_index] | (m_charbank << 8);
            int attr = (m_colorram[tile_index] & 0x1f) | (m_colortablebank << 5) | (m_palettebank << 6 );

            //SET_TILE_INFO_MEMBER(0,code,attr,0);
            tileinfo.set(0, (UInt32)code, (UInt32)attr, 0);
        }


        void init_save_state()
        {
            save_item(m_charbank, "m_charbank");
            save_item(m_spritebank, "m_spritebank");
            save_item(m_palettebank, "m_palettebank");
            save_item(m_colortablebank, "m_colortablebank");
            save_item(m_flipscreen, "m_flipscreen");
            save_item(m_bgpriority, "m_bgpriority");
        }


        //VIDEO_START_MEMBER(pacman_state,pacman)
        public void video_start_pacman()
        {
            init_save_state();

            m_charbank = 0;
            m_spritebank = 0;
            m_palettebank = 0;
            m_colortablebank = 0;
            m_flipscreen = 0;
            m_bgpriority = 0;
            m_inv_spr = 0;

            /* In the Pac Man based games (NOT Pengo) the first two sprites must be offset */
            /* one pixel to the left to get a more correct placement */
            m_xoffsethack = 1;

            m_bg_tilemap = machine().tilemap().create(m_gfxdecode.target.digfx, pacman_get_tile_info, pacman_scan_rows, 8, 8, 36, 28);
        }


        //WRITE8_MEMBER(pacman_state::pacman_videoram_w)
        public void pacman_videoram_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_videoram[offset] = data;
            m_bg_tilemap.mark_tile_dirty(offset);
        }


        //WRITE8_MEMBER(pacman_state::pacman_colorram_w)
        public void pacman_colorram_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_colorram[offset] = data;
            m_bg_tilemap.mark_tile_dirty(offset);
        }


        //WRITE_LINE_MEMBER(pacman_state::flipscreen_w)
        public void flipscreen_w(int state)
        {
            m_flipscreen = (byte)state;
            m_bg_tilemap.set_flip(m_flipscreen * ( tilemap_global.TILEMAP_FLIPX + tilemap_global.TILEMAP_FLIPY ) );
        }


        public uint32_t screen_update_pacman(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            if (m_bgpriority != 0)
                bitmap.fill(0, cliprect);
            else
                m_bg_tilemap.draw(screen, bitmap, cliprect, tilemap_global.TILEMAP_DRAW_OPAQUE, 0);

            if ( m_spriteram != null )
            {
                ListBytesPointer spriteram = m_spriteram.target;  //uint8_t *spriteram = m_spriteram;
                ListBytesPointer spriteram_2 = m_spriteram2.target;  //uint8_t *spriteram_2 = m_spriteram2;
                int offs;

                rectangle spriteclip = new rectangle(2*8, 34*8-1, 0*8, 28*8-1);
                spriteclip.intersection(cliprect); // spriteclip &= cliprect;

                /* Draw the sprites. Note that it is important to draw them exactly in this */
                /* order, to have the correct priorities. */
                for (offs = (int)m_spriteram.bytes() - 2; offs > 2 * 2; offs -= 2)
                {
                    int color;
                    int sx;
                    int sy;
                    byte fx;
                    byte fy;

                    if (m_inv_spr != 0)
                    {
                        sx = spriteram_2[offs + 1];
                        sy = 240 - (spriteram_2[offs]);
                    }
                    else
                    {
                        sx = 272 - spriteram_2[offs + 1];
                        sy = spriteram_2[offs] - 31;
                    }

                    fx = (byte)((spriteram[offs] & 1) ^ m_inv_spr);
                    fy = (byte)((spriteram[offs] & 2) ^ ((m_inv_spr) << 1));

                    color = ( spriteram[offs + 1] & 0x1f ) | (m_colortablebank << 5) | (m_palettebank << 6 );

                    m_gfxdecode.target.digfx.gfx(1).transmask(bitmap,spriteclip,
                            (UInt32)(( spriteram[offs] >> 2 ) | (m_spritebank << 6)),
                            (UInt32)color,
                            fx,fy,
                            sx,sy,
                            m_palette.target.palette_interface().transpen_mask(m_gfxdecode.target.digfx.gfx(1), (UInt32)color & 0x3f, 0));

                    /* also plot the sprite with wraparound (tunnel in Crush Roller) */
                    m_gfxdecode.target.digfx.gfx(1).transmask(bitmap,spriteclip,
                            (UInt32)(( spriteram[offs] >> 2 ) | (m_spritebank << 6)),
                            (UInt32)color,
                            fx,fy,
                            sx - 256,sy,
                            m_palette.target.palette_interface().transpen_mask(m_gfxdecode.target.digfx.gfx(1), (UInt32)color & 0x3f, 0));
                }

                /* In the Pac Man based games (NOT Pengo) the first two sprites must be offset */
                /* one pixel to the left to get a more correct placement */
                for (offs = 2 * 2; offs >= 0; offs -= 2)
                {
                    int color;
                    int sx;
                    int sy;
                    byte fx;
                    byte fy;

                    if (m_inv_spr != 0)
                    {
                        sx = spriteram_2[offs + 1];
                        sy = 240 - (spriteram_2[offs]);
                    }
                    else
                    {
                        sx = 272 - spriteram_2[offs + 1];
                        sy = spriteram_2[offs] - 31;
                    }

                    color = ( spriteram[offs + 1] & 0x1f ) | (m_colortablebank << 5) | (m_palettebank << 6 );

                    fx = (byte)((spriteram[offs] & 1) ^ m_inv_spr);
                    fy = (byte)((spriteram[offs] & 2) ^ ((m_inv_spr) << 1));

                    m_gfxdecode.target.digfx.gfx(1).transmask(bitmap,spriteclip,
                            (UInt32)(( spriteram[offs] >> 2 ) | (m_spritebank << 6)),
                            (UInt32)color,
                            fx,fy,
                            sx,sy + m_xoffsethack,
                            m_palette.target.palette_interface().transpen_mask(m_gfxdecode.target.digfx.gfx(1), (UInt32)color & 0x3f, 0));

                    /* also plot the sprite with wraparound (tunnel in Crush Roller) */
                    m_gfxdecode.target.digfx.gfx(1).transmask(bitmap,spriteclip,
                            (UInt32)(( spriteram[offs] >> 2 ) | (m_spritebank << 6)),
                            (UInt32)color,
                            fx,fy,
                            sx - 256,sy + m_xoffsethack,
                            m_palette.target.palette_interface().transpen_mask(m_gfxdecode.target.digfx.gfx(1), (UInt32)color & 0x3f, 0));
                }
            }

            if (m_bgpriority != 0)
                m_bg_tilemap.draw(screen, bitmap, cliprect, 0,0);

            return 0;
        }
    }
}