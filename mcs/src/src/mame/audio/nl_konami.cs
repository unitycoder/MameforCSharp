// license:BSD-3-Clause
// copyright-holders:Edward Fast

namespace mame
{
    partial class galaxian_state : driver_device
    {
        //static NETLIST_START(filter)
        void netlist_filter(netlist.nlparse_t setup)
        {
            NETLIST_START(setup);

            CD4066_GATE("G1");
            PARAM("G1.BASER", 270.0);
            CD4066_GATE("G2");
            PARAM("G2.BASER", 270.0);
            RES("RI", RES_K(1));
            RES("RO", RES_K(5));
            CAP("C1", CAP_U(0.22));
            CAP("C2", CAP_U(0.047));
            NET_C("RI.2", "RO.1", "G1.R.1", "G2.R.1");
            NET_C("G1.R.2", "C1.1");
            NET_C("G2.R.2", "C2.1");

            NET_C("C1.2", "C2.2", "G1.VSS", "G2.VSS");
            NET_C("G1.VDD", "G2.VDD");

            ALIAS("I", "RI.1");
            ALIAS("O", "RO.2");

            ALIAS("CTL1", "G1.CTL");
            ALIAS("CTL2", "G2.CTL");

            ALIAS("VDD", "G1.VDD");
            ALIAS("VSS", "G1.VSS");

            NETLIST_END();
        }


        //static NETLIST_START(amp)
        void netlist_amp(netlist.nlparse_t setup)
        {
            NETLIST_START(setup);

            UA741_DIP8("X3A");
            RES("R1", RES_K(2.2));
            RES("R2", RES_K(4.7));
            RES("VR", 200);         // Actually a potentiometer
            CAP("C1", CAP_U(0.15));
            RES("RI", RES_K(100));

            NET_C("X3A.2", "R1.1");
            NET_C("X3A.6", "R1.2", "R2.1");
            NET_C("R2.2", "VR.1");
            NET_C("VR.1", "C1.1");    // 100% pot position
            NET_C("C1.2", "RI.1");

            NET_C("GND", "VR.2", "RI.2");

            // Amplifier M51516L, assume input RI 100k

            ALIAS("OPAMP", "X3A.2");
            ALIAS("OUT", "RI.1");
            ALIAS("VP", "X3A.7");
            ALIAS("VM", "X3A.4");
            ALIAS("GND", "X3A.3");

            NETLIST_END();
        }


        //static NETLIST_START(AY1)
        void netlist_AY1(netlist.nlparse_t setup)
        {
            NETLIST_START(setup);

            TTL_INPUT("CTL0", 0);
            TTL_INPUT("CTL1", 0);
            TTL_INPUT("CTL2", 0);
            TTL_INPUT("CTL3", 0);
            TTL_INPUT("CTL4", 0);
            TTL_INPUT("CTL5", 0);
            /* AY 8910 internal resistors */
            RES("R_AY3D_A", 1000);
            RES("R_AY3D_B", 1000);
            RES("R_AY3D_C", 1000);
            NET_C("VP5", "R_AY3D_A.1", "R_AY3D_B.1", "R_AY3D_C.1");

            SUBMODEL("filter", "FCHA1");
            NET_C("FCHA1.I", "R_AY3D_A.2");
            SUBMODEL("filter", "FCHB1");
            NET_C("FCHB1.I", "R_AY3D_B.2");
            SUBMODEL("filter", "FCHC1");
            NET_C("FCHC1.I", "R_AY3D_C.2");

            NET_C("FCHA1.CTL1", "CTL0");
            NET_C("FCHA1.CTL2", "CTL1");
            NET_C("FCHB1.CTL1", "CTL2");
            NET_C("FCHB1.CTL2", "CTL3");
            NET_C("FCHC1.CTL1", "CTL4");
            NET_C("FCHC1.CTL2", "CTL5");

            NET_C("VP5", "FCHA1.VDD", "FCHB1.VDD", "FCHC1.VDD");
            NET_C("GND", "FCHA1.VSS", "FCHB1.VSS", "FCHC1.VSS");

            NET_C("VP5", "CTL0.VCC", "CTL1.VCC", "CTL2.VCC", "CTL3.VCC", "CTL4.VCC", "CTL5.VCC");
            NET_C("GND", "CTL0.GND", "CTL1.GND", "CTL2.GND", "CTL3.GND", "CTL4.GND", "CTL5.GND");

            NETLIST_END();
        }


        //static NETLIST_START(AY2)
        void netlist_AY2(netlist.nlparse_t setup)
        {
            NETLIST_START(setup);

            TTL_INPUT("CTL6", 0);
            TTL_INPUT("CTL7", 0);
            TTL_INPUT("CTL8", 0);
            TTL_INPUT("CTL9", 0);
            TTL_INPUT("CTL10", 0);
            TTL_INPUT("CTL11", 0);
            /* AY 8910 internal resistors */
            RES("R_AY3C_A", 1000);
            RES("R_AY3C_B", 1000);
            RES("R_AY3C_C", 1000);
            NET_C("VP5", "R_AY3C_A.1", "R_AY3C_B.1", "R_AY3C_C.1");

            SUBMODEL("filter", "FCHA2");
            NET_C("FCHA2.I", "R_AY3C_A.2");
            SUBMODEL("filter", "FCHB2");
            NET_C("FCHB2.I", "R_AY3C_B.2");
            SUBMODEL("filter", "FCHC2");
            NET_C("FCHC2.I", "R_AY3C_C.2");

            NET_C("FCHA2.CTL1", "CTL6");
            NET_C("FCHA2.CTL2", "CTL7");
            NET_C("FCHB2.CTL1", "CTL8");
            NET_C("FCHB2.CTL2", "CTL9");
            NET_C("FCHC2.CTL1", "CTL10");
            NET_C("FCHC2.CTL2", "CTL11");

            NET_C("VP5", "FCHA2.VDD", "FCHB2.VDD", "FCHC2.VDD");
            NET_C("GND", "FCHA2.VSS", "FCHB2.VSS", "FCHC2.VSS");

            NET_C("VP5", "CTL6.VCC", "CTL7.VCC", "CTL8.VCC", "CTL9.VCC", "CTL10.VCC", "CTL11.VCC");
            NET_C("GND", "CTL6.GND", "CTL7.GND", "CTL8.GND", "CTL9.GND", "CTL10.GND", "CTL11.GND");

            NETLIST_END();
        }


        //NETLIST_START(konami2x)
        void netlist_konami2x(netlist.nlparse_t setup)
        {
            NETLIST_START(setup);

            SOLVER("Solver", 48000);

            ANALOG_INPUT("VP5", 5);
            ANALOG_INPUT("VM5", -5);

            LOCAL_SOURCE("filter", netlist_filter);
            LOCAL_SOURCE("amp", netlist_amp);
            LOCAL_SOURCE("AY1", netlist_AY1);
            LOCAL_SOURCE("AY2", netlist_AY2);

            INCLUDE("AY1");
            INCLUDE("AY2");

            NET_C("FCHA1.O", "FCHB1.O", "FCHC1.O", "FCHA2.O", "FCHB2.O", "FCHC2.O");

            SUBMODEL("amp", "AMP");

            NET_C("VP5", "AMP.VP");
            NET_C("GND", "AMP.GND");
            NET_C("VM5", "AMP.VM");
            NET_C("FCHA1.O", "AMP.OPAMP");

            ALIAS("OUT", "AMP.OUT");

            NETLIST_END();
        }


        //NETLIST_START(konami1x)
        void netlist_konami1x(netlist.nlparse_t setup)
        {
            NETLIST_START(setup);

            SOLVER("Solver", 48000);

            ANALOG_INPUT("VP5", 5);
            ANALOG_INPUT("VM5", -5);

            LOCAL_SOURCE("filter", netlist_filter);
            LOCAL_SOURCE("amp", netlist_amp);
            LOCAL_SOURCE("AY1", netlist_AY1);
            LOCAL_SOURCE("AY2", netlist_AY2);

            INCLUDE("AY1");

            NET_C("FCHA1.O", "FCHB1.O", "FCHC1.O");

            SUBMODEL("amp", "AMP");

            NET_C("VP5", "AMP.VP");
            NET_C("GND", "AMP.GND");
            NET_C("VM5", "AMP.VM");
            NET_C("FCHA1.O", "AMP.OPAMP");

            ALIAS("OUT", "AMP.OUT");

            NETLIST_END();
        }
    }
}
