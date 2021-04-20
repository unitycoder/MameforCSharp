// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    namespace devices
    {
        public static class net_lib_global
        {
            //#ifdef RES_R
            //#warning "Do not include rescap.h in a netlist environment"
            //#endif
            //#ifndef RES_R
            //#define RES_R(res) (res)
            public static double RES_K(double res) { return res * 1e3; }
            public static double RES_M(double res) { return res * 1e6; }
            //#define CAP_U(cap) ((cap) * 1e-6)
            //#define CAP_N(cap) ((cap) * 1e-9)
            public static double CAP_P(double cap) { return cap * 1e-12; }
            //#define IND_U(ind) ((ind) * 1e-6)
            //#define IND_N(ind) ((ind) * 1e-9)
            //#define IND_P(ind) ((ind) * 1e-12)
            //#endif


            //#define SOLVER(name, freq)                                                  \
            //        NET_REGISTER_DEVEXT(SOLVER, name, freq)
            public static void SOLVER(nlparse_t setup, string name, int freq)
            {
                nl_setup_global.NET_REGISTER_DEV(setup, "SOLVER", name);
                nl_setup_global.PARAM(setup, name + ".FREQ", freq);
            }


            static factory.list_t initialize_factory_factory = null;

            //#define xstr(s) # s

            //#define NETLIB_DEVICE_DECL(chip) extern factory::constructor_ptr_t decl_ ## chip;

            static void LIB_DECL(factory.constructor_ptr_t decl) { initialize_factory_factory.add(decl()); }  //#define LIB_DECL(decl) factory.add( decl () );
            static void LIB_ENTRY(string nic, factory.constructor_ptr_t decl) { LIB_DECL(decl); }  //#define LIB_ENTRY(nic) { NETLIB_DEVICE_DECL(nic); LIB_DECL(decl_ ## nic) }


            public static void initialize_factory(factory.list_t factory)
            {
                initialize_factory_factory = factory;

                LIB_ENTRY("R", analog.nld_R.decl_R);
                LIB_ENTRY("POT", analog.nld_POT.decl_POT);
                //LIB_ENTRY(POT2)
                LIB_ENTRY("C", analog.nld_C.decl_C);
                //LIB_ENTRY(L)
                //LIB_ENTRY(D)
                //LIB_ENTRY(Z)
                //LIB_ENTRY(VS)
                //LIB_ENTRY(CS)
                //LIB_ENTRY(VCVS)
                //LIB_ENTRY(VCCS)
                //LIB_ENTRY(CCCS)
                //LIB_ENTRY(CCVS)
                //LIB_ENTRY(LVCCS)
                LIB_ENTRY("opamp", analog.nld_opamp.decl_opamp);
                //LIB_ENTRY(nc_pin)
                //LIB_ENTRY(frontier)   // not intended to be used directly
                //LIB_ENTRY(function)   // only for macro devices - NO FEEDBACK loops
                //LIB_ENTRY(QBJT_EB)
                //LIB_ENTRY(QBJT_switch)
                //LIB_ENTRY(MOSFET)
                LIB_ENTRY("logic_input_ttl", nld_logic_input.decl_logic_input_ttl);
                LIB_ENTRY("logic_input", nld_logic_input.decl_logic_input);
                //LIB_ENTRY(logic_input8)
                LIB_ENTRY("analog_input", nld_analog_input.decl_analog_input);
                //LIB_ENTRY(log)
                //LIB_ENTRY(logD)
                //LIB_ENTRY(clock)
                //LIB_ENTRY(varclock)
                //LIB_ENTRY(extclock)
                //LIB_ENTRY(mainclock)
                LIB_ENTRY("gnd", nld_gnd.decl_gnd);
                LIB_ENTRY("netlistparams", nld_netlistparams.decl_netlistparams);
                LIB_ENTRY("solver", nld_solver.decl_solver);
                //LIB_ENTRY(sys_dsw1)
                //LIB_ENTRY(sys_dsw2)
                //LIB_ENTRY(sys_compd)
                //LIB_ENTRY(sys_noise_mt_u)
                //LIB_ENTRY(sys_noise_mt_n)
                //LIB_ENTRY(switch1)
                //LIB_ENTRY(switch2)
                //LIB_ENTRY(nicRSFF)
                //LIB_ENTRY(nicDelay)
                //LIB_ENTRY(2102A)
                //LIB_ENTRY(2716)
                //LIB_ENTRY(7448)
                //LIB_ENTRY(MK28000)
                //LIB_ENTRY(7450)
                //LIB_ENTRY(7473)
                //LIB_ENTRY(7473A)
                //LIB_ENTRY(7474)
                //LIB_ENTRY(7475)
                //LIB_ENTRY(7475_GATE)
                //LIB_ENTRY(7477_GATE)
                //LIB_ENTRY(7483)
                //LIB_ENTRY(7485)
                //LIB_ENTRY(7490)
                //LIB_ENTRY(7492)
                //LIB_ENTRY(7493)
                //LIB_ENTRY(7497)
                //LIB_ENTRY(74107)
                //LIB_ENTRY(74107A)
                //LIB_ENTRY(74113)
                //LIB_ENTRY(74113A)
                //LIB_ENTRY(74121)
                //LIB_ENTRY(74123)
                //LIB_ENTRY(74125)
                //LIB_ENTRY(74126)
                //LIB_ENTRY(74153)
                //LIB_ENTRY(74161)
                //LIB_ENTRY(74161_fixme)
                //LIB_ENTRY(74163)
                //LIB_ENTRY(74164)
                //LIB_ENTRY(74165)
                //LIB_ENTRY(74166)
                //LIB_ENTRY(74174)
                //LIB_ENTRY(74175)
                //LIB_ENTRY(74192)
                //LIB_ENTRY(74193)
                //LIB_ENTRY(74194)
                //LIB_ENTRY(74365)
                //LIB_ENTRY(74377_GATE)
                //LIB_ENTRY(74393)
                ////ENTRY(74279,              TTL_74279,              "") // only dip available
                //LIB_ENTRY(SN74LS629)
                //LIB_ENTRY(82S16)
                //LIB_ENTRY(82S115)
                //LIB_ENTRY(82S123)
                //LIB_ENTRY(82S126)
                //LIB_ENTRY(74S287)
                //LIB_ENTRY(8277)
                //LIB_ENTRY(9310)
                //LIB_ENTRY(9314)
                //LIB_ENTRY(9316)
                //LIB_ENTRY(9321)
                //LIB_ENTRY(9322)
                //LIB_ENTRY(9334)
                //LIB_ENTRY(AM2847)
                //// FIXME: duplicate?
                //LIB_ENTRY(CD4006)
                //LIB_ENTRY(CD4013)
                //LIB_ENTRY(CD4017)
                //LIB_ENTRY(CD4022)
                //LIB_ENTRY(CD4020)
                //LIB_ENTRY(CD4024)
                //LIB_ENTRY(CD4053_GATE)
                LIB_ENTRY("CD4066_GATE", nld_CD4066_GATE.decl_CD4066_GATE);
                //LIB_ENTRY(CD4316_GATE)
                //LIB_ENTRY(4538)
                //LIB_ENTRY(schmitt_trigger)
                //LIB_ENTRY(NE555)
                //LIB_ENTRY(NE555_dip)
                //LIB_ENTRY(MC1455P)
                //LIB_ENTRY(MC1455P_dip)
                //LIB_ENTRY(TMS4800)
                //LIB_ENTRY(TMS4800_dip)
                //LIB_ENTRY(r2r_dac)
                //LIB_ENTRY(tristate)
                //LIB_ENTRY(tristate3)
                //LIB_ENTRY(8277_dip)
                //LIB_ENTRY(9602)
                //LIB_ENTRY(9314_dip)
                //LIB_ENTRY(9334_dip)
                //LIB_ENTRY(AM2847_dip)
                //LIB_ENTRY(MM5837_dip)

                initialize_factory_factory = null;
            }
        }
    } //namespace devices
} // namespace netlist
