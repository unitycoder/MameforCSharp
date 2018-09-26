// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using offs_t = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;


namespace mame
{
    public static class namco51_global
    {
        public const bool VERBOSE = false;


        public static void MCFG_NAMCO_51XX_ADD(out device_t device, machine_config config, device_t owner, string tag, XTAL clock) { mconfig_global.MCFG_DEVICE_ADD(out device, config, owner, tag, namco_51xx_device.NAMCO_51XX, clock); }
        public static void MCFG_NAMCO_51XX_SCREEN(device_t device, string screen_tag) { ((namco_51xx_device)device).set_screen_tag(screen_tag); }
        public static void MCFG_NAMCO_51XX_INPUT_0_CB(device_t device, DEVCB_IOPORT ioport_desc_devcb) { ((namco_51xx_device)device).set_input_callback(0, ioport_desc_devcb); }  // DEVCB_##_devcb);
        public static void MCFG_NAMCO_51XX_INPUT_1_CB(device_t device, DEVCB_IOPORT ioport_desc_devcb) { ((namco_51xx_device)device).set_input_callback(1, ioport_desc_devcb); }  // DEVCB_##_devcb);
        public static void MCFG_NAMCO_51XX_INPUT_2_CB(device_t device, DEVCB_IOPORT ioport_desc_devcb) { ((namco_51xx_device)device).set_input_callback(2, ioport_desc_devcb); }  // DEVCB_##_devcb);
        public static void MCFG_NAMCO_51XX_INPUT_3_CB(device_t device, DEVCB_IOPORT ioport_desc_devcb) { ((namco_51xx_device)device).set_input_callback(3, ioport_desc_devcb); }  // DEVCB_##_devcb);
        public static void MCFG_NAMCO_51XX_OUTPUT_0_CB(device_t device, write8_delegate write8_devcb) { ((namco_51xx_device)device).set_output_callback(0, write8_devcb); }  // DEVCB_##_devcb);
        public static void MCFG_NAMCO_51XX_OUTPUT_1_CB(device_t device, write8_delegate write8_devcb) { ((namco_51xx_device)device).set_output_callback(1, write8_devcb); }  // DEVCB_##_devcb);
    }


    class namco_51xx_device : device_t
    {
        //DEFINE_DEVICE_TYPE(NAMCO_51XX, namco_51xx_device, "namco51", "Namco 51xx")
        static device_t device_creator_namco_51xx_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new namco_51xx_device(mconfig, tag, owner, clock); }
        public static readonly device_type NAMCO_51XX = DEFINE_DEVICE_TYPE(device_creator_namco_51xx_device, "namco51", "Namco 51xx");


        //ROM_START( namco_51xx )
        static readonly List<tiny_rom_entry> rom_namco_51xx = new List<tiny_rom_entry>()
        {
            ROM_REGION( 0x400, "mcu", 0 ),
            ROM_LOAD( "51xx.bin",     0x0000, 0x0400, util.hash_global.CRC("c2f57ef8") + util.hash_global.SHA1("50de79e0d6a76bda95ffb02fcce369a79e6abfec") ),
            ROM_END(),
        };


        // internal state
        required_device<mb88_cpu_device> m_cpu;
        required_device<screen_device> m_screen;
        devcb_read8 [] m_in = new devcb_read8[4];
        devcb_write8 [] m_out = new devcb_write8[2];

        int m_lastcoins;
        int m_lastbuttons;
        int m_credits;
        int [] m_coins = new int[2];
        int [] m_coins_per_cred = new int[2];
        int [] m_creds_per_coin = new int[2];
        int m_in_count;
        int m_mode;
        int m_coincred_mode;
        int m_remap_joy;


        namco_51xx_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, NAMCO_51XX, tag, owner, clock)
        {
            m_cpu = new required_device<mb88_cpu_device>(this, "mcu");
            m_screen = new required_device<screen_device>(this, finder_base.DUMMY_TAG);

            for (int i = 0; i < 4; i++)
                m_in[i] = new devcb_read8(this);

            for (int i = 0; i < 2; i++)
                m_out[i] = new devcb_write8(this);

            m_lastcoins = 0;
            m_lastbuttons = 0;
            m_mode = 0;
            m_coincred_mode = 0;
            m_remap_joy = 0;
        }


        public void set_screen_tag(string tag) { m_screen.set_tag(tag); }  //template <typename T> void set_screen_tag(T &&tag) { m_screen.set_tag(std::forward<T>(tag)); }

        public devcb_base set_input_callback(int N, DEVCB_IOPORT cb) { return m_in[N].set_callback(this, cb); }  //template <unsigned N, class Object> devcb_base &set_input_callback(Object &&cb) { return m_in[N].set_callback(std::forward<Object>(cb)); }
        public devcb_base set_output_callback(int N, write8_delegate cb) { return m_out[N].set_callback(this, cb); }  //template <unsigned N, class Object> devcb_base &set_output_callback(Object &&cb) { return m_out[N].set_callback(std::forward<Object>(cb)); }
        //template <unsigned N> auto input_callback() { return m_in[N].bind(); }
        //template <unsigned N> auto output_callback() { return m_out[N].bind(); }


        //#define READ_PORT(num)           m_in[num](space, 0)
        public byte READ_PORT(int num, address_space space) { return m_in[num].op(space, 0); } 

        //#define WRITE_PORT(num, data)    m_out[num](space, 0, data)
        public void WRITE_PORT(int num, byte data, address_space space) { m_out[num].op(space, 0, data); }


        static game_driver write_namcoio_51XX_driver = null;
        static int write_namcoio_51XX_kludge = 0;

        //WRITE8_MEMBER( namco_51xx_device::write )
        public void write(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            data &= 0x07;

            if (namco51_global.VERBOSE) logerror("{0}: custom 51XX write {1}\n", machine().describe_context(), data);

            if (m_coincred_mode != 0)
            {
                switch (m_coincred_mode--)
                {
                    case 4: m_coins_per_cred[0] = data; break;
                    case 3: m_creds_per_coin[0] = data; break;
                    case 2: m_coins_per_cred[1] = data; break;
                    case 1: m_creds_per_coin[1] = data; break;
                }
            }
            else
            {
                switch (data)
                {
                    case 0: // nop
                        break;

                    case 1: // set coinage
                        m_coincred_mode = 4;
                        /* this is a good time to reset the credits counter */
                        m_credits = 0;

                        {
                            /* kludge for a possible bug in Xevious */
                            //static const game_driver *namcoio_51XX_driver = NULL;
                            //static int namcoio_51XX_kludge = 0;

                            /* Only compute namcoio_51XX_kludge when gamedrv changes */
                            if (write_namcoio_51XX_driver != machine().system())
                            {
                                write_namcoio_51XX_driver = machine().system();
                                if (write_namcoio_51XX_driver.name == "xevious" ||
                                    write_namcoio_51XX_driver.parent == "xevious")
                                    write_namcoio_51XX_kludge = 1;
                                else
                                    write_namcoio_51XX_kludge = 0;
                            }

                            if (write_namcoio_51XX_kludge != 0)
                            {
                                m_coincred_mode = 6;
                                m_remap_joy = 1;
                            }
                        }
                        break;

                    case 2: // go in "credits" mode and enable start buttons
                        m_mode = 1;
                        m_in_count = 0;
                        break;

                    case 3: // disable joystick remapping
                        m_remap_joy = 0;
                        break;

                    case 4: // enable joystick remapping
                        m_remap_joy = 1;
                        break;

                    case 5: // go in "switch" mode
                        m_mode = 0;
                        m_in_count = 0;
                        break;

                    default:
                        logerror("unknown 51XX command {0}\n", data);
                        break;
                }
            }
        }


        /* joystick input mapping

          The joystick is parsed and a number corresponding to the direction is returned,
          according to the following table:

                  0
                7   1
              6   8   2
                5   3
                  4

          The values for directions impossible to obtain on a joystick have not been
          verified on Namco original hardware, but they are the same in all the bootlegs,
          so we can assume they are right.
        */
        static readonly int [] joy_map = new int[16]
        /*  LDRU, LDR, LDU,  LD, LRU, LR,  LU,    L, DRU,  DR,  DU,   D,  RU,   R,   U, center */
        {    0xf, 0xe, 0xd, 0x5, 0xc, 0x9, 0x7, 0x6, 0xb, 0x3, 0xa, 0x4, 0x1, 0x2, 0x0, 0x8 };


        //READ8_MEMBER( namco_51xx_device::read )
        public u8 read(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            if (namco51_global.VERBOSE) logerror("{0}: custom 51XX read\n", machine().describe_context());

            if (m_mode == 0) /* switch mode */
            {
                switch ((m_in_count++) % 3)
                {
                    default:
                    case 0: return (byte)(READ_PORT(0, space) | (READ_PORT(1, space) << 4));
                    case 1: return (byte)(READ_PORT(2, space) | (READ_PORT(3, space) << 4));
                    case 2: return 0;   // nothing?
                }
            }
            else    /* credits mode */
            {
                switch ((m_in_count++) % 3)
                {
                    default:
                    case 0: // number of credits in BCD format
                        {
                            int in_;
                            int toggle;

                            in_ = ~(READ_PORT(0, space) | (READ_PORT(1, space) << 4));
                            toggle = in_ ^ m_lastcoins;
                            m_lastcoins = in_;

                            if (m_coins_per_cred[0] > 0)
                            {
                                if (m_credits >= 99)
                                {
                                    WRITE_PORT(1,1, space);  // coin lockout
                                }
                                else
                                {
                                    WRITE_PORT(1,0, space);  // coin lockout
                                    /* check if the user inserted a coin */
                                    if ((toggle & in_ & 0x10) != 0)
                                    {
                                        m_coins[0]++;
                                        WRITE_PORT(0,0x04, space);   // coin counter
                                        WRITE_PORT(0,0x0c, space);
                                        if (m_coins[0] >= m_coins_per_cred[0])
                                        {
                                            m_credits += m_creds_per_coin[0];
                                            m_coins[0] -= m_coins_per_cred[0];
                                        }
                                    }
                                    if ((toggle & in_ & 0x20) != 0)
                                    {
                                        m_coins[1]++;
                                        WRITE_PORT(0,0x08, space);   // coin counter
                                        WRITE_PORT(0,0x0c, space);
                                        if (m_coins[1] >= m_coins_per_cred[1])
                                        {
                                            m_credits += m_creds_per_coin[1];
                                            m_coins[1] -= m_coins_per_cred[1];
                                        }
                                    }
                                    if ((toggle & in_ & 0x40) != 0)
                                    {
                                        m_credits++;
                                    }
                                }
                            }
                            else m_credits = 100;    // free play

                            if (m_mode == 1)
                            {
                                // HACK: Just a way of deriving the lamp blink rate. Unclear if this is verified on actual hardware.
                                int on = ((int)m_screen.target.frame_number() & 0x10) >> 4;

                                if (m_credits >= 2)
                                    WRITE_PORT(0, (byte)(0x0c | 3 * on), space);    // lamps
                                else if (m_credits >= 1)
                                    WRITE_PORT(0, (byte)(0x0c | 2 * on), space);    // lamps
                                else
                                    WRITE_PORT(0,0x0c, space);   // lamps off

                                /* check for 1 player start button */
                                if ((toggle & in_ & 0x04) != 0)
                                {
                                    if (m_credits >= 1)
                                    {
                                        m_credits--;
                                        m_mode = 2;
                                        WRITE_PORT(0,0x0c, space);   // lamps off
                                    }
                                }
                                /* check for 2 players start button */
                                else if ((toggle & in_ & 0x08) != 0)
                                {
                                    if (m_credits >= 2)
                                    {
                                        m_credits -= 2;
                                        m_mode = 2;
                                        WRITE_PORT( 0,0x0c, space);  // lamps off
                                    }
                                }
                            }
                        }

                        if ((~READ_PORT( 1, space) & 0x08) != 0)    /* check test mode switch */
                            return 0xbb;

                        return (byte)((m_credits / 10) * 16 + m_credits % 10);

                    case 1:
                        {
                            int joy = READ_PORT(2, space) & 0x0f;
                            int in_;
                            int toggle;

                            in_ = ~READ_PORT(0, space);
                            toggle = in_ ^ m_lastbuttons;
                            m_lastbuttons = (m_lastbuttons & 2) | (in_ & 1);

                            /* remap joystick */
                            if (m_remap_joy != 0) joy = joy_map[joy];

                            /* fire */
                            joy |= ((toggle & in_ & 0x01)^1) << 4;
                            joy |= ((in_ & 0x01)^1) << 5;

                            return (byte)joy;
                        }

                    case 2:
                        {
                            int joy = READ_PORT(3, space) & 0x0f;
                            int in_;
                            int toggle;

                            in_ = ~READ_PORT(0, space);
                            toggle = in_ ^ m_lastbuttons;
                            m_lastbuttons = (m_lastbuttons & 1) | (in_ & 2);

                            /* remap joystick */
                            if (m_remap_joy != 0) joy = joy_map[joy];

                            /* fire */
                            joy |= ((toggle & in_ & 0x02)^2) << 3;
                            joy |= ((in_ & 0x02)^2) << 4;

                            return (byte)joy;
                        }
                }
            }
        }


        // device-level overrides

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            /* resolve our read callbacks */
            foreach (devcb_read8 cb in m_in)
                cb.resolve_safe(0);

            /* resolve our write callbacks */
            foreach (devcb_write8 cb in m_out)
                cb.resolve_safe();

            save_item(m_lastcoins, "m_lastcoins");
            save_item(m_lastbuttons, "m_lastbuttons");
            save_item(m_credits, "m_credits");
            save_item(m_coins, "m_coins");
            save_item(m_coins_per_cred, "m_coins_per_cred");
            save_item(m_creds_per_coin, "m_creds_per_coin");
            save_item(m_in_count, "m_in_count");
            save_item(m_mode, "m_mode");
            save_item(m_coincred_mode, "m_coincred_mode");
            save_item(m_remap_joy, "m_remap_joy");
        }

        //-------------------------------------------------
        //  device_reset - device-specific reset
        //-------------------------------------------------
        protected override void device_reset()
        {
            /* reset internal registers */
            m_credits = 0;
            m_coins[0] = 0;
            m_coins_per_cred[0] = 1;
            m_creds_per_coin[0] = 1;
            m_coins[1] = 0;
            m_coins_per_cred[1] = 1;
            m_creds_per_coin[1] = 1;
            m_in_count = 0;
        }

        //-------------------------------------------------
        //  device_rom_region - return a pointer to the
        //  the device's ROM definitions
        //-------------------------------------------------
        protected override List<tiny_rom_entry> device_rom_region()
        {
            return rom_namco_51xx;
        }

        //-------------------------------------------------
        //  device_add_mconfig - add device configuration
        //-------------------------------------------------
        protected override void device_add_mconfig(machine_config config, device_t owner, device_t device)
        {
            //MACHINE_CONFIG_START(namco_51xx_device::device_add_mconfig)
            MACHINE_CONFIG_START(config, owner, device);
                MCFG_DEVICE_ADD("mcu", mb8843_cpu_device.MB8843, DERIVED_CLOCK(1,1));     /* parent clock, internally divided by 6 */
                //  MCFG_MB88XX_READ_K_CB(READ8(*this, namco_51xx_device, namco_51xx_K_r))
                //  MCFG_MB88XX_WRITE_O_CB(WRITE8(*this, namco_51xx_device, namco_51xx_O_w))
                //  MCFG_MB88XX_READ_R0_CB(READ8(*this, namco_51xx_device, namco_51xx_R0_r))
                //  MCFG_MB88XX_READ_R2_CB(READ8(*this, namco_51xx_device, namco_51xx_R2_r))
                MCFG_DEVICE_DISABLE();
            MACHINE_CONFIG_END();
        }
    }
}