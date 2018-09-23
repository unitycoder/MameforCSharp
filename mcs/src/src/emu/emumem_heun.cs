// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using u8 = System.Byte;


namespace mame
{
    // handler_entry_read_unmapped/handler_entry_write_unmapped

    // Logs an unmapped access

    //template<int Width, int AddrShift, int Endian>
    public class handler_entry_read_unmapped : handler_entry_read
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_read<Width, AddrShift, Endian>;

        public handler_entry_read_unmapped(int Width, int AddrShift, int Endian, address_space space) : base(Width, AddrShift, Endian, space, 0) { }
        //~handler_entry_read_unmapped() = default;


        //uX read(offs_t offset, uX mem_mask) override;
        public override u8 read(offs_t offset, u8 mem_mask)
        {
            throw new emu_unimplemented();
#if false
            if (inh::m_space->log_unmap() && !inh::m_space->m_manager.machine().side_effects_disabled())
                inh::m_space->device().logerror(inh::m_space->is_octal()
                                                ? "%s: unmapped %s memory read from %0*o & %0*o\n"
                                                : "%s: unmapped %s memory read from %0*X & %0*X\n",
                                                inh::m_space->m_manager.machine().describe_context(), inh::m_space->name(),
                                                inh::m_space->addrchars(), offset,
                                                2 << Width, mem_mask);
            return inh::m_space->unmap();
#endif
        }


        //std::string name() const override;
    }


    //template<int Width, int AddrShift, int Endian>
    public class handler_entry_write_unmapped : handler_entry_write
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_write<Width, AddrShift, Endian>;

        public handler_entry_write_unmapped(int Width, int AddrShift, int Endian, address_space space) : base(Width, AddrShift, Endian, space, 0) { }
        //~handler_entry_write_unmapped() = default;


        public override void write(offs_t offset, u8 data, u8 mem_mask)
        {
            if (m_space.log_unmap() && !m_space.manager().machine().side_effects_disabled())
                m_space.device().logerror(m_space.is_octal()
                                                ? "{0}: unmapped {1} memory write to {2} = {3} & {4}\n"  // %0*o = %0*o & %0*o\n"
                                                : "{0}: unmapped {1} memory write to {2} = {3} & {4}\n",  // %0*X = %0*X & %0*X\n",
                                                m_space.manager().machine().describe_context(), m_space.name(),
                                                m_space.addrchars(), offset,
                                                2 << Width, data,
                                                2 << Width, mem_mask);
        }


        //std::string name() const override;
    }



    // handler_entry_read_nop/handler_entry_write_nop

    // Drops an unmapped access silently

    //template<int Width, int AddrShift, int Endian>
    class handler_entry_read_nop : handler_entry_read
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_read<Width, AddrShift, Endian>;

        public handler_entry_read_nop(int Width, int AddrShift, int Endian, address_space space) : base(Width, AddrShift, Endian, space, 0) { }
        //~handler_entry_read_nop() = default;


        //uX read(offs_t offset, uX mem_mask) override;
        public override u8 read(offs_t offset, u8 mem_mask)
        {
            return (u8)m_space.unmap();
        }


        //std::string name() const override;
    }


    //template<int Width, int AddrShift, int Endian>
    class handler_entry_write_nop : handler_entry_write
    {
        //using uX = typename emu::detail::handler_entry_size<Width>::uX;
        //using inh = handler_entry_write<Width, AddrShift, Endian>;

        public handler_entry_write_nop(int Width, int AddrShift, int Endian, address_space space) : base(Width, AddrShift, Endian, space, 0) { }
        //~handler_entry_write_nop() = default;


        public override void write(offs_t offset, u8 data, u8 mem_mask)
        {
        }


        //std::string name() const override;
    }
}