// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public enum iodevice_t
    {
        /* List of all supported devices.  Refer to the device by these names only */
        IO_UNKNOWN,
        IO_CARTSLOT,    /*  1 - Cartridge Port, as found on most console and on some computers */
        IO_FLOPPY,      /*  2 - Floppy Disk unit */
        IO_HARDDISK,    /*  3 - Hard Disk unit */
        IO_CYLINDER,    /*  4 - Magnetically-Coated Cylinder */
        IO_CASSETTE,    /*  5 - Cassette Recorder (common on early home computers) */
        IO_PUNCHCARD,   /*  6 - Card Puncher/Reader */
        IO_PUNCHTAPE,   /*  7 - Tape Puncher/Reader (reels instead of punchcards) */
        IO_PRINTER,     /*  8 - Printer device */
        IO_SERIAL,      /*  9 - Generic Serial Port */
        IO_PARALLEL,    /* 10 - Generic Parallel Port */
        IO_SNAPSHOT,    /* 11 - Complete 'snapshot' of the state of the computer */
        IO_QUICKLOAD,   /* 12 - Allow to load program/data into memory, without matching any actual device */
        IO_MEMCARD,     /* 13 - Memory card */
        IO_CDROM,       /* 14 - optical CD-ROM disc */
        IO_MAGTAPE,     /* 15 - Magentic tape */
        IO_ROM,         /* 16 - Individual ROM image - the Amstrad CPC has a few applications that were sold on 16kB ROMs */
        IO_MIDIIN,      /* 17 - MIDI In port */
        IO_MIDIOUT,     /* 18 - MIDI Out port */
        IO_COUNT        /* 19 - Total Number of IO_devices for searching */
    }


    enum image_error_t
    {
        IMAGE_ERROR_SUCCESS,
        IMAGE_ERROR_INTERNAL,
        IMAGE_ERROR_UNSUPPORTED,
        IMAGE_ERROR_OUTOFMEMORY,
        IMAGE_ERROR_FILENOTFOUND,
        IMAGE_ERROR_INVALIDIMAGE,
        IMAGE_ERROR_ALREADYOPEN,
        IMAGE_ERROR_UNSPECIFIED
    }


    public enum image_init_result { PASS, FAIL }
    public enum image_verify_result { PASS, FAIL }


    // ======================> device_image_interface
    // class representing interface-specific live image
    public abstract class device_image_interface : device_interface
    {
        //typedef std::vector<std::unique_ptr<image_device_format>> formatlist_type;


        //static const image_device_type_info m_device_info_array[];


        /* error related info */
        image_error_t m_err;
        string m_err_message;

        /* variables that are only non-zero when an image is mounted */
        //core_file *m_file;
        //emu_file *m_mame_file;
        string m_image_name;
        //astring m_basename;
        string m_basename_noext;
        //astring m_filetype;

        /* Software information */
        string m_full_software_name;
        software_part m_software_part_ptr;
        string m_software_list_name;

        // creation info
        //formatlist_type m_formatlist;

        // working directory; persists across mounts
        //std::string m_working_directory;

        /* info read from the hash file/software list */
        //astring m_longname;
        //astring m_manufacturer;
        //astring m_year;
        //UINT32  m_supported;

        /* flags */
        //bool m_readonly;
        //bool m_created;

        /* special - used when creating */
        //int m_create_format;
        //option_resolution *m_create_args;

        //hash_collection m_hash;

        string m_instance_name;                // e.g. - "cartridge", "floppydisk2"
        string m_brief_instance_name;          // e.g. - "cart", "flop2"
        string m_canonical_instance_name;      // e.g. - "cartridge1", "floppydisk2" - only used internally in emuopts.cpp

        /* in the case of arcade cabinet with fixed carts inserted,
         we want to disable command line cart loading... */
        bool m_user_loadable;

        bool m_is_loading;

        bool m_is_reset_and_loading;


        // construction/destruction
        //-------------------------------------------------
        //  device_image_interface - constructor
        //-------------------------------------------------
        public device_image_interface(machine_config mconfig, device_t device)
            : base(device, "image")
        {
            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  device_typename - retrieves device type name
        //-------------------------------------------------
        public static string device_typename(iodevice_t type)
        {
            throw new emu_unimplemented();
        }

        //static const char *device_brieftypename(iodevice_t type);
        //static iodevice_t device_typeid(const char *name);


        //virtual image_init_result call_load() { return FALSE; }
        //virtual image_init_result call_create(int format_type, option_resolution *format_options) { return FALSE; }
        //virtual void call_unload() { }
        public virtual string call_display() { return ""; }
        //virtual u32 unhashed_header_length() const { return 0; }
        //virtual bool core_opens_image_file() const { return TRUE; }
        public abstract iodevice_t image_type();
        protected abstract bool is_readable();
        protected abstract bool is_writeable();
        protected abstract bool is_creatable();
        public abstract bool must_be_loaded();
        public abstract bool is_reset_on_load();

        //-------------------------------------------------
        //  support_command_line_image_creation - do we
        //  want to support image creation from the front
        //  end command line?
        //-------------------------------------------------
        protected virtual bool support_command_line_image_creation()
        {
            bool result;
            switch (image_type())
            {
            case iodevice_t.IO_PRINTER:
            case iodevice_t.IO_SERIAL:
            case iodevice_t.IO_PARALLEL:
                // going by the assumption that these device image types should support this
                // behavior; ideally we'd get rid of IO_* and just push this to the specific
                // devices
                result = true;
                break;
            default:
                result = false;
                break;
            }

            return result;
        }


        public abstract string image_interface();
        public abstract string file_extensions();
        //protected abstract option_guide create_option_guide();
        //virtual const char *custom_instance_name() const { return nullptr; }
        //virtual const char *custom_brief_instance_name() const { return nullptr; }


        //const image_device_format *device_get_indexed_creatable_format(int index) const { if (index < m_formatlist.size()) return m_formatlist.at(index).get(); else return nullptr;  }
        //const image_device_format *device_get_named_creatable_format(const char *format_name);
        //const option_guide *device_get_creation_option_guide() { return create_option_guide(); }


        /*-------------------------------------------------
            error - returns the error text for an image
            error
        -------------------------------------------------*/
        static readonly string [] messages = new string[]
        {
            "",
            "Internal error",
            "Unsupported operation",
            "Out of memory",
            "File not found",
            "Invalid image",
            "File already open",
            "Unspecified error"
        };

        public string error()
        {
            return !string.IsNullOrEmpty(m_err_message) ? m_err_message : messages[(int)m_err];
        }


        //void seterror(image_error_t err, const char *message);
        //void message(const char *format, ...) ATTR_PRINTF(2,3);


        public bool exists() { return !string.IsNullOrEmpty(m_image_name); }
        public string filename() { if (string.IsNullOrEmpty(m_image_name)) return null; else return m_image_name; }
        //const char *basename() { if (!m_basename) return NULL; else return m_basename; }
        public string basename_noext()  { if (string.IsNullOrEmpty(m_basename_noext)) return null; else return m_basename_noext; }
        //const std::string &filetype() const { return m_filetype; }
        //bool is_filetype(const std::string &candidate_filetype) { return !core_stricmp(filetype().c_str(), candidate_filetype.c_str()); }
        //bool is_open() const { return bool(m_file); }
        //core_file *image_core_file() { return m_file; }
        //UINT64 length() { check_for_file(); return core_fsize(m_file); }
        //bool is_readonly() { return m_readonly; }
        //UINT32 fread(void *buffer, UINT32 length) { check_for_file(); return core_fread(m_file, buffer, length); }
        //UINT32 fread(optional_shared_ptr<UINT8> &ptr, UINT32 length) { ptr.allocate(length); return fread(ptr.target(), length); }
        //UINT32 fread(optional_shared_ptr<UINT8> &ptr, UINT32 length, offs_t offset) { ptr.allocate(length); return fread(ptr + offset, length - offset); }
        //UINT32 fwrite(const void *buffer, UINT32 length) { check_for_file(); return core_fwrite(m_file, buffer, length); }
        //int fseek(INT64 offset, int whence) { check_for_file(); return core_fseek(m_file, offset, whence); }
        //UINT64 ftell() { check_for_file(); return core_ftell(m_file); }
        //int fgetc() { char ch; if (fread(&ch, 1) != 1) ch = '\0'; return ch; }
        //char *fgets(char *buffer, UINT32 length) { check_for_file(); return core_fgets(buffer, length, m_file); }
        //int image_feof() { check_for_file(); return core_feof(m_file); }
        //void *ptr() {check_for_file(); return (void *) core_fbuffer(m_file); }


        // configuration access

        //const char* longname() { return m_longname; }
        //const char* manufacturer() { return m_manufacturer; }
        //const char* year() { return m_year; }
        //UINT32 supported() { return m_supported; }


        public software_info software_entry() { return (m_software_part_ptr == null) ? null : m_software_part_ptr.info(); }
        public software_part part_entry() { return m_software_part_ptr; }
        public string software_list_name() { return m_software_list_name; }
        public bool loaded_through_softlist() { return m_software_part_ptr != null; }


        //void set_working_directory(const char *working_directory) { m_working_directory = working_directory; }
        //const char * working_directory();


        //UINT8 *get_software_region(const char *tag);
        //UINT32 get_software_region_length(const char *tag);
        //const char *get_feature(const char *feature_name);
        //bool load_software_region(const char *tag, optional_shared_ptr<UINT8> &ptr);


        //UINT32 crc();
        //hash_collection& hash() { return m_hash; }
        //util::hash_collection calculate_hash_on_file(util::core_file &file) const;


        //void battery_load(void *buffer, int length, int fill);
        //void battery_load(void *buffer, int length, void *def_buffer);
        //void battery_save(const void *buffer, int length);

        //const char *image_type_name()  const { return device_typename(image_type()); }



        public string instance_name() { return m_instance_name; }
        public string brief_instance_name() { return m_brief_instance_name; }
        //const std::string &cannonical_instance_name() const { return m_canonical_instance_name; }
        //bool uses_file_extension(const char *file_extension) const;
        //formatlist_type formatlist() const { return m_formatlist.first(); }


        // loads an image file
        //image_init_result load(const std::string &path);

        // loads a softlist item by name
        //image_init_result load_software(const std::string &software_identifier);


        /*-------------------------------------------------
            image_finish_load - special call - only use
            from core
        -------------------------------------------------*/
        public image_init_result finish_load()
        {
            throw new emu_unimplemented();
        }


        /*-------------------------------------------------
            unload - main call to unload an image
        -------------------------------------------------*/
        public void unload()
        {
            throw new emu_unimplemented();
        }


        //image_init_result create(const char *path, const image_device_format *create_format, option_resolution *create_args);
        //image_init_result create(const std::string &path);
        //bool load_software(software_list_device &swlist, const char *swname, const rom_entry *entry);
        //int reopen_for_write(const char *path);


        //void set_user_loadable(bool user_loadable) { m_user_loadable = user_loadable; }


        public bool user_loadable() { return m_user_loadable; }
        public bool is_reset_and_loading() { return m_is_reset_and_loading; }
        public string full_software_name() { return m_full_software_name; }


        // interface-level overrides

        //-------------------------------------------------
        //  interface_config_complete - perform any
        //  operations now that the configuration is
        //  complete
        //-------------------------------------------------
        public override void interface_config_complete()
        {
            // set brief and instance name
            update_names();
        }


        //virtual const software_list_loader &get_software_list_loader() const;
        //virtual const bool use_software_list_file_extension_for_filetype() const { return false; }
        //image_init_result load_internal(const std::string &path, bool is_create, int create_format, util::option_resolution *create_args);
        //void determine_open_plan(int is_create, UINT32 *open_plan);
        //image_error_t load_image_by_path(UINT32 open_flags, const char *path);
        //void clear();
        //bool is_loaded();


        //void set_image_filename(const std::string &filename);


        //void clear_error();


        //void check_for_file() { assert_always(m_file != NULL, "Illegal operation on unmounted image"); }


        //void setup_working_directory();
        //bool try_change_working_directory(const char *subdir);


        //void make_readonly() { m_readonly = true; }


        //bool image_checkhash();


        //software_part *find_software_item(const char *path, bool restrict_to_interface);
        //string software_get_default_slot(const char *default_card_slot);


        //void add_format(std::unique_ptr<image_device_format> &&format);
        //void add_format(std::string &&name, std::string &&description, std::string &&extensions, std::string &&optspec);


        // derived class overrides

        // configuration
        //static const image_device_type_info *find_device_type(iodevice_t type);


        //static image_error_t image_error_from_file_error(osd_file::error filerr);
        //std::vector<u32> determine_open_plan(bool is_create);

        //-------------------------------------------------
        //  update_names - update brief and instance names
        //-------------------------------------------------
        void update_names()
        {
            throw new emu_unimplemented();
        }


        //bool load_software_part(const std::string &identifier);

        //bool init_phase() const;
        //static bool run_hash(util::core_file &file, u32 skip_bytes, util::hash_collection &hashes, const char *types);

        // loads an image or software items and resets - called internally when we
        // load an is_reset_on_load() item
        //void reset_and_load(const std::string &path);
    }


    // iterator
    //typedef device_interface_iterator<device_image_interface> image_interface_iterator;
    public class image_interface_iterator : device_interface_iterator<device_image_interface>
    {
        public image_interface_iterator(device_t root, int maxdepth = 255) : base(root, maxdepth) { }
    }
}
