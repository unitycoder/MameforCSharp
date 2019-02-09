// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using offs_t = System.UInt32;
using u8 = System.Byte;
using uint8_t = System.Byte;


namespace mame
{
    partial class _1942_state : driver_device
    {
        /* memory pointers */
        required_shared_ptr_uint8_t m_spriteram;
        required_shared_ptr_uint8_t m_fg_videoram;
        required_shared_ptr_uint8_t m_bg_videoram;

        required_device<cpu_device> m_audiocpu;
        required_device<cpu_device> m_maincpu;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<palette_device> m_palette;
        required_device<generic_latch_8_device> m_soundlatch;

        /* video-related */
        tilemap_t m_fg_tilemap;
        tilemap_t m_bg_tilemap;
        int m_palette_bank;
        uint8_t [] m_scroll = new uint8_t[2];


        public _1942_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_spriteram = new required_shared_ptr_uint8_t(this, "spriteram");
            m_fg_videoram = new required_shared_ptr_uint8_t(this, "fg_videoram");
            m_bg_videoram = new required_shared_ptr_uint8_t(this, "bg_videoram");
            m_audiocpu = new required_device<cpu_device>(this, "audiocpu");
            m_maincpu = new required_device<cpu_device>(this, "maincpu");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_palette = new required_device<palette_device>(this, "palette");
            m_soundlatch = new required_device<generic_latch_8_device>(this, "soundlatch");
        }


        public required_device<palette_device> palette { get { return m_palette; } }
        public required_device<generic_latch_8_device> soundlatch { get { return m_soundlatch; } }


        //void driver_init() override;

        //TILE_GET_INFO_MEMBER(get_fg_tile_info);
        //TILE_GET_INFO_MEMBER(get_bg_tile_info);

        //void _1942(machine_config &config);

        //void machine_start() override;
        //void machine_reset() override;
        //void video_start() override;

        //void _1942_map(address_map &map);
        //void sound_map(address_map &map);

        //DECLARE_WRITE8_MEMBER(_1942_bankswitch_w);
        //DECLARE_WRITE8_MEMBER(_1942_fgvideoram_w);
        //DECLARE_WRITE8_MEMBER(_1942_bgvideoram_w);
        //DECLARE_WRITE8_MEMBER(_1942_palette_bank_w);
        //DECLARE_WRITE8_MEMBER(_1942_scroll_w);
        //DECLARE_WRITE8_MEMBER(_1942_c804_w);
        //void _1942_palette(palette_device &palette) const;
        //TIMER_DEVICE_CALLBACK_MEMBER(_1942_scanline);
        //uint32_t screen_update(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect);
        //virtual void draw_sprites(bitmap_ind16 &bitmap, const rectangle &cliprect);


        // wrappers because I don't know how to find the correct device during construct_ startup

        //READ8_MEMBER( generic_latch_8_device::read )
        public byte generic_latch_8_device_read(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            generic_latch_8_device device = (generic_latch_8_device)subdevice("soundlatch");
            return device.read(space, offset, mem_mask);
        }

        //WRITE8_MEMBER( generic_latch_8_device::write )
        public void generic_latch_8_device_write(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            generic_latch_8_device device = (generic_latch_8_device)subdevice("soundlatch");
            device.write(space, offset, data, mem_mask);
        }

        //WRITE8_MEMBER( ay8910_device::data_w )
        public void ay8910_device_address_data_w_ay1(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            ay8910_device device = (ay8910_device)subdevice("ay1");
            device.data_w(space, offset, data, mem_mask);
        }

        //WRITE8_MEMBER( ay8910_device::data_w )
        public void ay8910_device_address_data_w_ay2(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            ay8910_device device = (ay8910_device)subdevice("ay2");
            device.data_w(space, offset, data, mem_mask);
        }
    }
}
