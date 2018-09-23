// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using ListBytesPointer = mame.ListPointer<System.Byte>;
using stream_sample_t = System.Int32;
using u8 = System.Byte;
using u32 = System.UInt32;


namespace mame
{
    public static class disound_global
    {
        public const int ALL_OUTPUTS       = 65535;    // special value indicating all outputs for the current chip
        public const int AUTO_ALLOC_INPUT  = 65535;


        //**************************************************************************
        //  INTERFACE CONFIGURATION MACROS
        //**************************************************************************

        //#define MCFG_SOUND_ROUTE(_output, _target, ...)     dynamic_cast<device_sound_interface &>(*device).add_route(_output, _target, __VA_ARGS__);
        public static void MCFG_SOUND_ROUTE(device_t device, u32 output, string target, double gain) { device.GetClassInterface<device_sound_interface>().add_route(output, target, gain); }
        public static void MCFG_SOUND_ROUTE(device_t device, u32 output, string target, double gain, u32 input) { device.GetClassInterface<device_sound_interface>().add_route(output, target, gain, input); }
        //define MCFG_SOUND_ROUTES_RESET()             device_sound_interface::static_reset_routes(*device);

        //define MCFG_MIXER_ROUTE(_output, _target, _gain, _mixoutput)             device_sound_interface::static_add_route(*device, _output, _target, _gain, AUTO_ALLOC_INPUT, _mixoutput);
    }


    // ======================> device_sound_interface
    public abstract class device_sound_interface : device_interface
    {
        public class sound_route
        {
            public u32 m_output;           // output index, or ALL_OUTPUTS
            public u32 m_input;            // target input index
            public u32 m_mixoutput;        // target mixer output
            public float m_gain;             // gain
            public device_t m_base;  //std::reference_wrapper<device_t>    m_base;             // target search base
            public string m_target;           // target tag

            public sound_route(u32 output, u32 input, u32 mixoutput, float gain, device_t base_, string target)
            {
                m_output = output;
                m_input = input;
                m_mixoutput = mixoutput;
                m_gain = gain;
                m_base = base_;
                m_target = target;
            }
        }


        // internal state
        std_vector<sound_route> m_route_list = new std_vector<sound_route>();      // list of sound routes
        int m_outputs;                  // number of outputs from this instance
        protected int m_auto_allocated_inputs;    // number of auto-allocated inputs targeting us


        // construction/destruction
        //-------------------------------------------------
        //  device_sound_interface - constructor
        //-------------------------------------------------
        public device_sound_interface(machine_config mconfig, device_t device)
            : base(device, "sound")
        {
        }


        public virtual bool issound() { return true; } /// HACK: allow devices to hide from the ui


        // configuration access
        public ListBase<sound_route> routes() { return m_route_list; }


        // configuration helpers

        //-------------------------------------------------
        //  add_route - send sound output to a consumer
        //-------------------------------------------------
        public device_sound_interface add_route(u32 output, string target, double gain, u32 input = disound_global.AUTO_ALLOC_INPUT, u32 mixoutput = 0)
        {
            global.assert(!device().started());
            m_route_list.emplace_back(new sound_route(output, input, mixoutput, (float)gain, device().mconfig().current_device(), target));
            return this;
        }

        public device_sound_interface add_route(u32 output, device_sound_interface target, double gain, u32 input = disound_global.AUTO_ALLOC_INPUT, u32 mixoutput = 0)
        {
            global.assert(!device().started());
            m_route_list.emplace_back(new sound_route(output, input, mixoutput, (float)gain, target.device(), device_global.DEVICE_SELF));
            return this;
        }

        public device_sound_interface add_route(u32 output, speaker_device target, double gain, u32 input = disound_global.AUTO_ALLOC_INPUT, u32 mixoutput = 0)
        {
            global.assert(!device().started());
            m_route_list.emplace_back(new sound_route(output, input, mixoutput, (float)gain, target, device_global.DEVICE_SELF));
            return this;
        }


        //device_sound_interface &reset_routes() { m_route_list.clear(); return *this; }


        // sound stream update overrides
        public abstract void sound_stream_update(sound_stream stream, ListPointer<stream_sample_t> [] inputs, ListPointer<stream_sample_t> [] outputs, int samples);


        // stream creation
        //-------------------------------------------------
        //  stream_alloc - allocate a stream implicitly
        //  associated with this device
        //-------------------------------------------------
        public sound_stream stream_alloc(int inputs, int outputs, int sample_rate)
        {
            return device().machine().sound().stream_alloc(device(), inputs, outputs, sample_rate);
        }


        // helpers

        //-------------------------------------------------
        //  inputs - return the total number of inputs
        //  for the given device
        //-------------------------------------------------
        int inputs()
        {
            // scan the list counting streams we own and summing their inputs
            int inputs = 0;
            foreach (var stream in device().machine().sound().streams())
            {
                if (stream.device() == device())
                    inputs += stream.input_count();
            }

            return inputs;
        }

        //-------------------------------------------------
        //  outputs - return the total number of outputs
        //  for the given device
        //-------------------------------------------------
        public int outputs()
        {
            // scan the list counting streams we own and summing their outputs
            int outputs = 0;
            foreach (var stream in device().machine().sound().streams())
            {
                if (stream.device() == device())
                    outputs += stream.output_count();
            }

            return outputs;
        }

        //-------------------------------------------------
        //  input_to_stream_input - convert a device's
        //  input index to a stream and the input index
        //  on that stream
        //-------------------------------------------------
        sound_stream input_to_stream_input(int inputnum, out int stream_inputnum)
        {
            global.assert(inputnum >= 0);

            stream_inputnum = -1;

            // scan the list looking for streams owned by this device
            foreach (var stream in device().machine().sound().streams())
            {
                if (stream.device() == device())
                {
                    if (inputnum < stream.input_count())
                    {
                        stream_inputnum = inputnum;
                        return stream;
                    }

                    inputnum -= stream.input_count();
                }
            }

            // not found
            return null;
        }

        //-------------------------------------------------
        //  output_to_stream_output - convert a device's
        //  output index to a stream and the output index
        //  on that stream
        //-------------------------------------------------
        sound_stream output_to_stream_output(int outputnum, out int stream_outputnum)
        {
            global.assert(outputnum >= 0);

            stream_outputnum = -1;

            // scan the list looking for streams owned by this device
            foreach (var stream in device().machine().sound().streams())
            {
                if (stream.device() == device())
                {
                    if (outputnum < stream.output_count())
                    {
                        stream_outputnum = outputnum;
                        return stream;
                    }

                    outputnum -= stream.output_count();
                }
            }

            // not found
            return null;
        }

        //void set_input_gain(int inputnum, float gain);
        //void set_output_gain(int outputnum, float gain);
        //int inputnum_from_device(device_t &device, int outputnum = 0) const;


        // configuration access
        //std::vector<sound_route> &routes() { return m_route_list; }


        // optional operation overrides

        //-------------------------------------------------
        //  interface_validity_check - validation for a
        //  device after the configuration has been
        //  constructed
        //-------------------------------------------------
        protected override void interface_validity_check(validity_checker valid)
        {
            // loop over all the routes
            foreach (sound_route route in routes())
            {
                // find a device with the requested tag
                device_t target = route.m_base.get().subdevice(route.m_target.c_str());
                if (target == null)
                    global.osd_printf_error("Attempting to route sound to non-existent device '{0}'\n", route.m_base.get().subtag(route.m_target.c_str()).c_str());

                // if it's not a speaker or a sound device, error
                device_sound_interface sound;
                if (target != null && (target.type() != speaker_device.SPEAKER) && !target.interface_(out sound))
                    global.osd_printf_error("Attempting to route sound to a non-sound device '{0}' ({1})\n", target.tag(), target.name());
            }
        }

        //-------------------------------------------------
        //  interface_pre_start - make sure all our input
        //  devices are started
        //-------------------------------------------------
        public override void interface_pre_start()
        {
            // scan all the sound devices
            sound_interface_iterator iter = new sound_interface_iterator(device().machine().root_device());
            foreach (device_sound_interface sound in iter)
            {
                // scan each route on the device
                foreach (sound_route route in sound.routes())
                {
                    // see if we are the target of this route; if we are, make sure the source device is started
                    device_t target_device = route.m_base.get().subdevice(route.m_target.c_str());
                    if ((target_device == device()) && !sound.device().started())
                        throw new device_missing_dependencies();
                }
            }

            // now iterate through devices again and assign any auto-allocated inputs
            m_auto_allocated_inputs = 0;
            foreach (device_sound_interface sound in iter)
            {
                // scan each route on the device
                foreach (sound_route route in sound.routes())
                {
                    // see if we are the target of this route
                    device_t target_device = route.m_base.get().subdevice(route.m_target.c_str());
                    if ((target_device == device()) && (route.m_input == disound_global.AUTO_ALLOC_INPUT))
                    {
                        route.m_input = (UInt32)m_auto_allocated_inputs;
                        m_auto_allocated_inputs += (route.m_output == disound_global.ALL_OUTPUTS) ? sound.outputs() : 1;
                    }
                }
            }
        }

        //-------------------------------------------------
        //  interface_post_start - verify that state was
        //  properly set up
        //-------------------------------------------------
        public override void interface_post_start()
        {
            // iterate over all the sound devices
            foreach (device_sound_interface sound in new sound_interface_iterator(device().machine().root_device()))
            {
                // scan each route on the device
                foreach (sound_route route in sound.routes())
                {
                    // if we are the target of this route, hook it up
                    device_t target_device = route.m_base.get().subdevice(route.m_target.c_str());
                    if (target_device == device())
                    {
                        // iterate over all outputs, matching any that apply
                        int inputnum = (int)route.m_input;
                        int numoutputs = sound.outputs();
                        for (int outputnum = 0; outputnum < numoutputs; outputnum++)
                        {
                            if ((route.m_output == outputnum) || (route.m_output == disound_global.ALL_OUTPUTS))
                            {
                                // find the output stream to connect from
                                int streamoutputnum;
                                sound_stream outputstream = sound.output_to_stream_output(outputnum, out streamoutputnum);
                                if (outputstream == null)
                                    global.fatalerror("Sound device '{0}' specifies route for non-existent output #{1}\n", sound.device().tag(), outputnum);

                                // find the input stream to connect to
                                int streaminputnum;
                                sound_stream inputstream = input_to_stream_input(inputnum++, out streaminputnum);
                                if (inputstream == null)
                                    global.fatalerror("Sound device '{0}' targeted output #{1} to non-existant device '{2}' input {3}\n", sound.device().tag(), outputnum, device().tag(), inputnum - 1);

                                // set the input
                                inputstream.set_input(streaminputnum, outputstream, streamoutputnum, route.m_gain);
                            }
                        }
                    }
                }
            }
        }

        //-------------------------------------------------
        //  interface_pre_reset - called prior to
        //  resetting the device
        //-------------------------------------------------
        public override void interface_pre_reset()
        {
            // update all streams on this device prior to reset
            foreach (var stream in device().machine().sound().streams())
            {
                if (stream.device() == device())
                    stream.update();
            }
        }
    }


    // iterator
    //typedef device_interface_iterator<device_sound_interface> sound_interface_iterator;
    public class sound_interface_iterator : device_interface_iterator<device_sound_interface>
    {
        public sound_interface_iterator(device_t root, int maxdepth = 255) : base(root, maxdepth) { }
    }


    // ======================> device_mixer_interface
    public class device_mixer_interface : device_sound_interface
    {
        // internal state
        u8 m_outputs;              // number of outputs
        std_vector<u8> m_outputmap = new std_vector<u8>();   // map of inputs to outputs
        sound_stream m_mixer_stream;         // mixing stream


        // construction/destruction
        //-------------------------------------------------
        //  device_mixer_interface - constructor
        //-------------------------------------------------
        public device_mixer_interface(machine_config mconfig, device_t device, int outputs = 1)
            : base(mconfig, device)
        {
            m_outputs = (byte)outputs;
            m_mixer_stream = null;
        }


        // getters
        public sound_stream mixer_stream() { return m_mixer_stream; }


        // optional operation overrides

        //-------------------------------------------------
        //  interface_pre_start - perform startup prior
        //  to the device startup
        //-------------------------------------------------
        public override void interface_pre_start()
        {
            // call our parent
            base.interface_pre_start();

            // no inputs? that's weird
            if (m_auto_allocated_inputs == 0)
            {
                device().logerror("Warning: mixer \"{0}\" has no inputs\n", device().tag());
                return;
            }

            // generate the output map
            m_outputmap.resize(m_auto_allocated_inputs);

            // iterate through all routes that point to us and note their mixer output
            foreach (device_sound_interface sound in new sound_interface_iterator(device().machine().root_device()))
            {
                foreach (sound_route route in sound.routes())
                {
                    // see if we are the target of this route
                    device_t target_device = route.m_base.get().subdevice(route.m_target.c_str());
                    if ((target_device == device()) && (route.m_input < m_auto_allocated_inputs))
                    {
                        int count = (route.m_output == disound_global.ALL_OUTPUTS) ? sound.outputs() : 1;
                        for (int output = 0; output < count; output++)
                            m_outputmap[(int)(route.m_input + output)] = (byte)route.m_mixoutput;
                    }
                }
            }

            // allocate the mixer stream
            m_mixer_stream = stream_alloc(m_auto_allocated_inputs, m_outputs, device().machine().sample_rate());
        }

        //-------------------------------------------------
        //  interface_post_load - after we load a save
        //  state be sure to update the mixer stream's
        //  output sample rate
        //-------------------------------------------------
        public override void interface_post_load()
        {
            // Beware that there's not going to be a mixer stream if there was
            // no inputs
            if (m_mixer_stream != null)
                m_mixer_stream.set_sample_rate(device().machine().sample_rate());

            // call our parent
            base.interface_post_load();
        }


        // sound interface overrides
        //-------------------------------------------------
        //  mixer_update - mix all inputs to one output
        //-------------------------------------------------
        public override void sound_stream_update(sound_stream stream, ListPointer<stream_sample_t> [] inputs, ListPointer<stream_sample_t> [] outputs, int samples)
        {
            // clear output buffers
            for (int output = 0; output < m_outputs; output++)
            {
                global.memset(outputs[output], 0, (UInt32)samples);  //memset(outputs[output], 0, samples * sizeof(outputs[0][0]));
            }

            // loop over samples
            ListBytesPointer outmap = new ListBytesPointer(m_outputmap);  //const u8 *outmap = &m_outputmap[0];
            for (int pos = 0; pos < samples; pos++)
            {
                // for each input, add it to the appropriate output
                for (int inp = 0; inp < m_auto_allocated_inputs; inp++)
                    outputs[outmap[inp]][pos] += inputs[inp][pos];
            }
        }
    }


    // iterator
    //typedef device_interface_iterator<device_mixer_interface> mixer_interface_iterator;
    public class mixer_interface_iterator : device_interface_iterator<device_mixer_interface>
    {
        public mixer_interface_iterator(device_t root, int maxdepth = 255) : base(root, maxdepth) { }
    }
}