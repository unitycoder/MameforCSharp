// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class nlm_cd4xxx_global
    {
        /* ----------------------------------------------------------------------------
         *  Netlist Macros
         * ---------------------------------------------------------------------------*/
        //#define CD4001_NOR(name)                                                      \
        //        NET_REGISTER_DEV(CD4001_NOR, name)
        public static void CD4001_NOR(netlist.setup_t setup, string name) { netlist.nl_setup_global.NET_REGISTER_DEV(setup, "CD4001_NOR", name); }

        //#define CD4001_DIP(name)                                                      \
        //        NET_REGISTER_DEV(CD4001_DIP, name)

        /* ----------------------------------------------------------------------------
         *  DIP only macros
         * ---------------------------------------------------------------------------*/
        //#define CD4020_DIP(name)                                                      \
        //        NET_REGISTER_DEV(CD4020_DIP, name)

        //#define CD4066_DIP(name)                                                      \
        //        NET_REGISTER_DEV(CD4066_DIP, name)

        //#define CD4016_DIP(name)                                                      \
        //        NET_REGISTER_DEV(CD4016_DIP, name)

        //#define CD4316_DIP(name)                                                      \
        //        NET_REGISTER_DEV(CD4016_DIP, name)


        /*
         *   CD4001BC: Quad 2-Input NOR Buffered B Series Gate
         *
         *       +--------------+
         *    A1 |1     ++    14| VCC
         *    B1 |2           13| A6
         *    A2 |3           12| Y6
         *    Y2 |4    4001   11| A5
         *    A3 |5           10| Y5
         *    Y3 |6            9| A4
         *   GND |7            8| Y4
         *       +--------------+
         *
         */

        //static NETLIST_START(CD4001_DIP)
        public static void nld_CD4001_DIP(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            CD4001_NOR(setup, "s1");
            CD4001_NOR(setup, "s2");
            CD4001_NOR(setup, "s3");
            CD4001_NOR(setup, "s4");

            netlist.devices.nld_system_global.DUMMY_INPUT(setup, "VSS");
            netlist.devices.nld_system_global.DUMMY_INPUT(setup, "VDD");

            netlist.nl_setup_global.DIPPINS(setup,    /*       +--------------+      */
                "s1.A",   /*    A1 |1     ++    14| VCC  */ "VDD.I",
                "s1.B",   /*    B1 |2           13| A6   */ "s4.B",
                "s1.Q",   /*    A2 |3           12| Y6   */ "s4.A",
                "s2.Q",   /*    Y2 |4    4001   11| A5   */ "s4.Q",
                "s2.A",   /*    A3 |5           10| Y5   */ "s3.Q",
                "s2.B",   /*    Y3 |6            9| A4   */ "s3.B",
                "VSS.I",  /*   GND |7            8| Y4   */ "s3.A"
                        /*       +--------------+      */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        /*  CD4020: 14-Stage Ripple Carry Binary Counters
         *
         *          +--------------+
         *      Q12 |1     ++    16| VDD
         *      Q13 |2           15| Q11
         *      Q14 |3           14| Q10
         *       Q6 |4    4020   13| Q8
         *       Q5 |5           12| Q9
         *       Q7 |6           11| RESET
         *       Q4 |7           10| IP (Input pulses)
         *      VSS |8            9| Q1
         *          +--------------+
         *
         *  Naming conventions follow Texas Instruments datasheet
         *
         *  FIXME: Timing depends on VDD-VSS
         *         This needs a cmos d-a/a-d proxy implementation.
         */

        //static NETLIST_START(CD4020_DIP)
        public static void nld_CD4020_DIP(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_4020_global.CD4020(setup, "s1");
            netlist.nl_setup_global.DIPPINS(setup,     /*       +--------------+       */
                "s1.Q12",  /*   Q12 |1     ++    16| VDD   */ "s1.VDD",
                "s1.Q13",  /*   Q13 |2           15| Q11   */ "s1.Q11",
                "s1.Q14",  /*   Q14 |3           14| Q10   */ "s1.Q10",
                "s1.Q6",   /*    Q6 |4    4020   13| Q8    */ "s1.Q8",
                "s1.Q5",   /*    Q5 |5           12| Q9    */ "s1.Q9",
                "s1.Q7",   /*    Q7 |6           11| RESET */ "s1.RESET",
                "s1.Q4",   /*    Q4 |7           10| IP    */ "s1.IP",
                "s1.VSS",  /*   VSS |8            9| Q1    */ "s1.Q1"
                            /*       +--------------+       */
            );
                /*
                 * IP = (Input pulses)
                 */

            netlist.nl_setup_global.NETLIST_END();
        }


        /*  CD4066: Quad Bilateral Switch
         *
         *          +--------------+
         *   INOUTA |1     ++    14| VDD
         *   OUTINA |2           13| CONTROLA
         *   OUTINB |3           12| CONTROLD
         *   INOUTB |4    4066   11| INOUTD
         * CONTROLB |5           10| OUTIND
         * CONTROLC |6            9| OUTINC
         *      VSS |7            8| INOUTC
         *          +--------------+
         *
         *  FIXME: These devices are slow (~125 ns). THis is currently not reflected
         *
         *  Naming conventions follow National semiconductor datasheet
         *
         */
        //static NETLIST_START(CD4066_DIP)
        public static void nld_CD4066_DIP(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_4066_global.CD4066_GATE(setup, "A");
            nld_4066_global.CD4066_GATE(setup, "B");
            nld_4066_global.CD4066_GATE(setup, "C");
            nld_4066_global.CD4066_GATE(setup, "D");

            netlist.nl_setup_global.NET_C(setup, "A.PS.VDD", "B.PS.VDD", "C.PS.VDD", "D.PS.VDD");
            netlist.nl_setup_global.NET_C(setup, "A.PS.VSS", "B.PS.VSS", "C.PS.VSS", "D.PS.VSS");

            netlist.nl_setup_global.PARAM(setup, "A.BASER", 270.0);
            netlist.nl_setup_global.PARAM(setup, "B.BASER", 270.0);
            netlist.nl_setup_global.PARAM(setup, "C.BASER", 270.0);
            netlist.nl_setup_global.PARAM(setup, "D.BASER", 270.0);

            netlist.nl_setup_global.DIPPINS(setup,        /*          +--------------+          */
                "A.R.1",      /*   INOUTA |1     ++    14| VDD      */ "A.PS.VDD",
                "A.R.2",      /*   OUTINA |2           13| CONTROLA */ "A.CTL",
                "B.R.1",      /*   OUTINB |3           12| CONTROLD */ "D.CTL",
                "B.R.2",      /*   INOUTB |4    4066   11| INOUTD   */ "D.R.1",
                "B.CTL",      /* CONTROLB |5           10| OUTIND   */ "D.R.2",
                "C.CTL",      /* CONTROLC |6            9| OUTINC   */ "C.R.1",
                "A.PS.VSS",   /*      VSS |7            8| INOUTC   */ "C.R.2"
                            /*          +--------------+          */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(CD4016_DIP)
        public static void nld_CD4016_DIP(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_4066_global.CD4066_GATE(setup, "A");
            nld_4066_global.CD4066_GATE(setup, "B");
            nld_4066_global.CD4066_GATE(setup, "C");
            nld_4066_global.CD4066_GATE(setup, "D");

            netlist.nl_setup_global.NET_C(setup, "A.PS.VDD", "B.PS.VDD", "C.PS.VDD", "D.PS.VDD");
            netlist.nl_setup_global.NET_C(setup, "A.PS.VSS", "B.PS.VSS", "C.PS.VSS", "D.PS.VSS");

            netlist.nl_setup_global.PARAM(setup, "A.BASER", 1000.0);
            netlist.nl_setup_global.PARAM(setup, "B.BASER", 1000.0);
            netlist.nl_setup_global.PARAM(setup, "C.BASER", 1000.0);
            netlist.nl_setup_global.PARAM(setup, "D.BASER", 1000.0);

            netlist.nl_setup_global.DIPPINS(setup,        /*          +--------------+          */
                "A.R.1",      /*   INOUTA |1     ++    14| VDD      */ "A.PS.VDD",
                "A.R.2",      /*   OUTINA |2           13| CONTROLA */ "A.CTL",
                "B.R.1",      /*   OUTINB |3           12| CONTROLD */ "D.CTL",
                "B.R.2",      /*   INOUTB |4    4016   11| INOUTD   */ "D.R.1",
                "B.CTL",      /* CONTROLB |5           10| OUTIND   */ "D.R.2",
                "C.CTL",      /* CONTROLC |6            9| OUTINC   */ "C.R.1",
                "A.PS.VSS",   /*      VSS |7            8| INOUTC   */ "C.R.2"
                            /*          +--------------+          */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //static NETLIST_START(CD4316_DIP)
        public static void nld_CD4316_DIP(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            nld_4316_global.CD4316_GATE(setup, "A");
            nld_4316_global.CD4316_GATE(setup, "B");
            nld_4316_global.CD4316_GATE(setup, "C");
            nld_4316_global.CD4316_GATE(setup, "D");

            netlist.nl_setup_global.NET_C(setup, "A.E", "B.E", "C.E", "D.E");
            netlist.nl_setup_global.NET_C(setup, "A.PS.VDD", "B.PS.VDD", "C.PS.VDD", "D.PS.VDD");
            netlist.nl_setup_global.NET_C(setup, "A.PS.VSS", "B.PS.VSS", "C.PS.VSS", "D.PS.VSS");

            netlist.nl_setup_global.PARAM(setup, "A.BASER", 45.0);
            netlist.nl_setup_global.PARAM(setup, "B.BASER", 45.0);
            netlist.nl_setup_global.PARAM(setup, "C.BASER", 45.0);
            netlist.nl_setup_global.PARAM(setup, "D.BASER", 45.0);

            netlist.nl_setup_global.DIPPINS(setup,        /*          +--------------+          */
                "A.R.2",      /*       1Z |1     ++    16| VCC      */ "A.PS.VDD",
                "A.R.1",      /*       1Y |2           15| 1S       */ "A.S",
                "B.R.1",      /*       2Y |3           14| 4S       */ "D.S",
                "B.R.2",      /*       2Z |4    4316   13| 4Z       */ "D.R.2",
                "B.S",        /*       2S |5           12| 4Y       */ "D.R.1",
                "C.S",        /*       3S |6           11| 3Y       */ "C.R.1",
                "A.E",        /*       /E |7           10| 3Z       */ "C.R.2",
                "A.PS.VSS",   /*      GND |8            9| VEE      */ "VEE"
                            /*          +--------------+          */
            );

            netlist.nl_setup_global.NETLIST_END();
        }


        //NETLIST_START(CD4XXX_lib)
        public static void nld_CD4XXX_lib(netlist.setup_t setup)
        {
            netlist.nl_setup_global.NETLIST_START();

            netlist.nl_setup_global.TRUTHTABLE_START("CD4001_NOR", 2, 1, "");
                netlist.nl_setup_global.TT_HEAD("A , B | Q ");
                netlist.nl_setup_global.TT_LINE("0,0|1|85");
                netlist.nl_setup_global.TT_LINE("X,1|0|120");
                netlist.nl_setup_global.TT_LINE("1,X|0|120");
                netlist.nl_setup_global.TT_FAMILY("CD4XXX");
            netlist.nl_setup_global.TRUTHTABLE_END(setup);

            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "CD4001_DIP", nld_CD4001_DIP);

            /* DIP ONLY */
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "CD4020_DIP", nld_CD4020_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "CD4016_DIP", nld_CD4016_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "CD4066_DIP", nld_CD4066_DIP);
            netlist.nl_setup_global.LOCAL_LIB_ENTRY(setup, "CD4316_DIP", nld_CD4316_DIP);

            netlist.nl_setup_global.NETLIST_END();
        }
    }
}