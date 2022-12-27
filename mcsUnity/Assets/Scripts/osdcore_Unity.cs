// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using mame;

using osd_ticks_t = System.UInt64;  //typedef uint64_t osd_ticks_t;

using static mame.osdcore_global;


namespace mcsUnity
{
    public class osd_file_Unity : osd_file
    {
        Stream m_stream = null;
        byte [] m_data = null;


        /*-----------------------------------------------------------------------------
            osd_file::open: open a new file.

            Parameters:

                path - path to the file to open

                openflags - some combination of:

                    OPEN_FLAG_READ - open the file for read access
                    OPEN_FLAG_WRITE - open the file for write access
                    OPEN_FLAG_CREATE - create/truncate the file when opening
                    OPEN_FLAG_CREATE_PATHS - specifies that non-existant paths
                            should be created if necessary

                file - reference to an osd_file::ptr to receive the newly-opened file
                    handle; this is only valid if the function returns FILERR_NONE

                filesize - reference to a UINT64 to receive the size of the opened
                    file; this is only valid if the function returns FILERR_NONE

            Return value:

                a file_error describing any error that occurred while opening
                the file, or FILERR_NONE if no error occurred

            Notes:

                This function is called by core_fopen and several other places in
                the core to access files. These functions will construct paths by
                concatenating various search paths held in the options.c options
                database with partial paths specified by the core. The core assumes
                that the path separator is the first character of the string
                PATH_SEPARATOR, but does not interpret any path separators in the
                search paths, so if you use a different path separator in a search
                path, you may get a mixture of PATH_SEPARATORs (from the core) and
                alternate path separators (specified by users and placed into the
                options database).
        -----------------------------------------------------------------------------*/
        public override std.error_condition open(string path, UInt32 openflags, out osd_file file, out UInt64 filesize)
        {
            // path "roms/digdug/dd1a.1" string

            file = this;
            try
            {
                m_data = null;

                string dirPath = Path.GetDirectoryName(path);
                string filename = Path.GetFileName(path);

                //UnityEngine.Debug.LogFormat("osdcore_Unity() path: {0}", dirPath);

                if (Main.m_synchronizeInvoke == null)
                {
                    filesize = 0;
                    return std.errc.no_such_file_or_directory;
                }

                IAsyncResult result = Main.m_synchronizeInvoke.BeginInvoke((System.Func<string>)(() =>
                {
                    UnityEngine.GameObject romGameObject = UnityEngine.GameObject.Find(dirPath);

                    if (romGameObject != null)
                    {
                        Rom romComponent = romGameObject.GetComponent<Rom>();
                        if (romComponent != null)
                        {
                            foreach (UnityEngine.TextAsset r in romComponent.m_roms)
                            {
                                if (r.name == filename)
                                {
                                    m_data = r.bytes;

                                    //UnityEngine.Debug.LogFormat("osdcore_Unity() - Found: {0}", path);
                                    break;
                                }
                            }
                        }
                    }

                    return "";
                }), null);

                Main.m_synchronizeInvoke.EndInvoke(result);

                for (int i = 0; i < 100; i++)
                {
                    if (result.IsCompleted)
                        break;

                    System.Threading.Thread.Sleep(10);
                }

                //UnityEngine.Debug.LogFormat("osd_file_Unity.open() - '{0}' {1} {2} {3}", retObj as string, m_position, m_path, m_name);

                if (m_data == null)
                {
                    filesize = 0;
                    return std.errc.no_such_file_or_directory;
                }

                m_stream = new MemoryStream(m_data);
                filesize = (UInt64)m_data.Length;
                return new std.error_condition();
            }
            catch (Exception)
            {
                filesize = 0;
                return std.errc.no_such_file_or_directory;
            }
        }


        public override void close()
        {
            throw new emu_unimplemented();
        }


        /*-----------------------------------------------------------------------------
            osd_file::openpty: create a new PTY pair

            Parameters:

                file - reference to an osd_file::ptr to receive the handle of the master
                    side of the newly-created PTY; this is only valid if the function
                    returns FILERR_NONE

                name - reference to string where slave filename will be stored

            Return value:

                a file_error describing any error that occurred while creating the
                PTY, or FILERR_NONE if no error occurred
        -----------------------------------------------------------------------------*/
        protected override std.error_condition openpty(out osd_file file, out string name) { throw new emu_unimplemented(); }


        /*-----------------------------------------------------------------------------
            osd_file::read: read from an open file

            Parameters:

                buffer - pointer to memory that will receive the data read

                offset - offset within the file to read from

                length - number of bytes to read from the file

                actual - reference to a UINT32 to receive the number of bytes actually
                    read during the operation; valid only if the function returns
                    FILERR_NONE

            Return value:

                a file_error describing any error that occurred while reading
                from the file, or FILERR_NONE if no error occurred
        -----------------------------------------------------------------------------*/
        public override std.error_condition read(Pointer<byte> buffer, UInt64 offset, UInt32 length, out UInt32 actual)  //virtual std::error_condition read(void *buffer, std::uint64_t offset, std::uint32_t length, std::uint32_t &actual) = 0;
        {
            //UnityEngine.Debug.LogFormat("osd_file_Unity.read() - {0} {1} {2} - {3} {4}", m_path, m_name, m_data.Length, offset, length);

            // read one byte at a time (HACK)
            UInt32 i = 0;
            for (; i < length; i++)
            {
                if (offset + i >= (UInt64)m_data.Length)
                    break;

                buffer[i] = m_data[offset + i];
            }

            actual = i;
            return new std.error_condition();
        }


        /*-----------------------------------------------------------------------------
            osd_file::write: write to an open file

            Parameters:

                buffer - pointer to memory that contains the data to write

                offset - offset within the file to write to

                length - number of bytes to write to the file

                actual - reference to a UINT32 to receive the number of bytes actually
                    written during the operation; valid only if the function returns
                    FILERR_NONE

            Return value:

                a file_error describing any error that occurred while writing to
                the file, or FILERR_NONE if no error occurred
        -----------------------------------------------------------------------------*/
        public override std.error_condition write(Pointer<byte> buffer, UInt64 offset, UInt32 length, out UInt32 actual) { throw new emu_unimplemented(); }  //virtual std::error_condition write(void const *buffer, std::uint64_t offset, std::uint32_t length, std::uint32_t &actual) = 0;


        /// \brief Change the size of an open file
        ///
        /// \param [in] offset Desired size of the file.
        /// \return Result of the operation.
        public override std.error_condition truncate(UInt64 offset) { throw new emu_unimplemented(); }


        /// \brief Flush file buffers
        ///
        /// This flushes any data cached by the application, but does not
        /// guarantee that all prior writes have reached persistent storage.
        /// \return Result of the operation.
        public override std.error_condition flush() { throw new emu_unimplemented(); }


        /*-----------------------------------------------------------------------------
            osd_file::remove: deletes a file

            Parameters:

                filename - path to file to delete

            Return value:

                a file_error describing any error that occurred while deleting
                the file, or FILERR_NONE if no error occurred
        -----------------------------------------------------------------------------*/
        public override std.error_condition remove(string filename) { throw new emu_unimplemented(); }


        public override Stream stream { get { return m_stream; } }
    }


    public class osd_directory_Unity : mame.osd.directory
    {
        public DirectoryInfo m_directory;  // info about the current directory
        public DirectoryInfo [] m_directories;  // list of directories in this directory
        public FileInfo [] m_files;
        public int m_directoryIdx = 0;
        public int m_fileIdx = 0;

        public override entry read()
        {
            if (m_directoryIdx < m_directories.Length)
            {
                entry newentry = new entry();
                newentry.name = m_directories[m_directoryIdx++].Name;
                newentry.type = entry.entry_type.DIR;
                return newentry;
            }
            else if (m_fileIdx < m_files.Length)
            {
                entry newentry = new entry();
                newentry.name = m_files[m_fileIdx++].Name;
                newentry.type = entry.entry_type.FILE;
                return newentry;
            }
            else
            {
                return null;
            }
        }
    }


    public class osd_directory_static_Unity : mame.osd.directory_static
    {
        public override mame.osd.directory open(string dirname)
        {
            if (!Directory.Exists(dirname))
                return null;

            osd_directory_Unity dirWinForms = new osd_directory_Unity();
            dirWinForms.m_directory = new DirectoryInfo(dirname);
            dirWinForms.m_directories = dirWinForms.m_directory.GetDirectories();
            dirWinForms.m_files = dirWinForms.m_directory.GetFiles();
            return dirWinForms;
        }
    }


    public class osdcore_Unity : osdcore_interface
    {
        /*-----------------------------------------------------------------------------
            osd_open: open a new file.

            Parameters:

                path - path to the file to open

                openflags - some combination of:

                    OPEN_FLAG_READ - open the file for read access
                    OPEN_FLAG_WRITE - open the file for write access
                    OPEN_FLAG_CREATE - create/truncate the file when opening
                    OPEN_FLAG_CREATE_PATHS - specifies that non-existant paths
                            should be created if necessary

                file - pointer to an osd_file * to receive the newly-opened file
                    handle; this is only valid if the function returns FILERR_NONE

                filesize - pointer to a UINT64 to receive the size of the opened
                    file; this is only valid if the function returns FILERR_NONE

            Return value:

                a file_error describing any error that occurred while opening
                the file, or FILERR_NONE if no error occurred

            Notes:

                This function is called by core_fopen and several other places in
                the core to access files. These functions will construct paths by
                concatenating various search paths held in the options.c options
                database with partial paths specified by the core. The core assumes
                that the path separator is the first character of the string
                PATH_SEPARATOR, but does not interpret any path separators in the
                search paths, so if you use a different path separator in a search
                path, you may get a mixture of PATH_SEPARATORs (from the core) and
                alternate path separators (specified by users and placed into the
                options database).
        -----------------------------------------------------------------------------*/
        public std.error_condition osd_open(string path, UInt32 openflags, out osd_file file, out UInt64 filesize)
        {
            throw new emu_fatalerror("TODO - Fix inheritence");

            //osd_file_Unity fileWinForms = new osd_file_Unity();
            //file = fileWinForms;
            //try
            //{
            //    //fileWinForms.m_file = File.OpenRead(path);
            //}
            //catch (Exception)
            //{
            //    filesize = 0;
            //    return osd_file.error.NOT_FOUND;
            //}
            //
            ////filesize = (UInt64)fileWinForms.m_file.Length;
            //return osd_file.error.NONE;
        }


        /*-----------------------------------------------------------------------------
            osd_close: close an open file

            Parameters:

                file - handle to a file previously opened via osd_open

            Return value:

                a file_error describing any error that occurred while closing
                the file, or FILERR_NONE if no error occurred
        -----------------------------------------------------------------------------*/
        public std.error_condition osd_close(ref osd_file file)
        {
            throw new emu_fatalerror("TODO - Fix inheritence");

            //osd_file_Unity fileWinForms = (osd_file_Unity)file;
            //
            ////if (fileWinForms.m_file != null)
            ////    fileWinForms.m_file.Close();
            //
            //file = null;
            //return osd_file.error.NONE;
        }


        /*-----------------------------------------------------------------------------
            osd_read: read from an open file

            Parameters:

                file - handle to a file previously opened via osd_open

                buffer - pointer to memory that will receive the data read

                offset - offset within the file to read from

                length - number of bytes to read from the file

                actual - pointer to a UINT32 to receive the number of bytes actually
                    read during the operation; valid only if the function returns
                    FILERR_NONE

            Return value:

                a file_error describing any error that occurred while reading
                from the file, or FILERR_NONE if no error occurred
        -----------------------------------------------------------------------------*/
        public std.error_condition osd_read(osd_file file, Pointer<byte> buffer, UInt64 offset, UInt32 length, out UInt32 actual)
        {
            throw new emu_fatalerror("TODO - Fix inheritence");

            //osd_file_Unity fileWinForms = (osd_file_Unity)file;
            //
            ////fileWinForms.m_file.Position = 0;
            //
            //// read until we get to the correct offset (HACK)
            ////while (offset-- > 0)
            ////    fileWinForms.m_file.ReadByte();
            //
            //// read one byte at a time (HACK)
            //for (UInt32 i = 0; i < length; i++)
            //{
            //    //buffer[i] = (byte)fileWinForms.m_file.ReadByte();
            //}
            //
            //actual = length;
            //return osd_file.error.NONE;
        }


#if false
        /*-----------------------------------------------------------------------------
            osd_opendir: open a directory for iteration

            Parameters:

                dirname - path to the directory in question

            Return value:

                upon success, this function should return an osd_directory pointer
                which contains opaque data necessary to traverse the directory; on
                failure, this function should return NULL
        -----------------------------------------------------------------------------*/
        public override osd_directory osd_opendir(string dirname)
        {
            if (!Directory.Exists(dirname))
                return null;

            osd_directory_WinForms dirWinForms = new osd_directory_WinForms();
            dirWinForms.m_directory = new DirectoryInfo(dirname);
            dirWinForms.m_directories = dirWinForms.m_directory.GetDirectories();
            dirWinForms.m_files = dirWinForms.m_directory.GetFiles();
            return dirWinForms;
        }


        /*-----------------------------------------------------------------------------
            osd_readdir: return information about the next entry in the directory

            Parameters:

                dir - pointer to an osd_directory that was returned from a prior
                    call to osd_opendir

            Return value:

                a constant pointer to an osd_directory_entry representing the current item
                in the directory, or NULL, indicating that no more entries are
                present
        -----------------------------------------------------------------------------*/
        public override osd_directory_entry osd_readdir(osd_directory dir)
        {
            osd_directory_WinForms dirWinForms = (osd_directory_WinForms)dir;

            if (dirWinForms.m_directoryIdx < dirWinForms.m_directories.Length)
            {
                osd_directory_entry entry = new osd_directory_entry();
                entry.name = dirWinForms.m_directories[dirWinForms.m_directoryIdx++].Name;
                entry.type = osd_dir_entry_type.ENTTYPE_DIR;
                return entry;
            }
            else if (dirWinForms.m_fileIdx < dirWinForms.m_files.Length)
            {
                osd_directory_entry entry = new osd_directory_entry();
                entry.name = dirWinForms.m_files[dirWinForms.m_fileIdx++].Name;
                entry.type = osd_dir_entry_type.ENTTYPE_FILE;
                return entry;
            }
            else
            {
                return null;
            }
        }


        /*-----------------------------------------------------------------------------
            osd_closedir: close an open directory for iteration

            Parameters:

                dir - pointer to an osd_directory that was returned from a prior
                    call to osd_opendir

            Return value:

                frees any allocated memory and resources associated with the open
                directory
        -----------------------------------------------------------------------------*/
        public override void osd_closedir(osd_directory dir)
        {
        }
#endif

        /*-----------------------------------------------------------------------------
            osd_sleep: sleep for the specified time interval

            Parameters:

                duration - an osd_ticks_t value that specifies how long we should
                    sleep

            Return value:

                None

            Notes:

                The OSD layer should try to sleep for as close to the specified
                duration as possible, or less. This is used as a mechanism to
                "give back" unneeded time to other programs running in the system.
                On a simple, non multitasking system, this routine can be a no-op.
                If there is significant volatility in the amount of time that the
                sleep occurs for, the OSD layer should strive to sleep for less time
                than specified rather than sleeping too long.
        -----------------------------------------------------------------------------*/
        public override void osd_sleep(osd_ticks_t duration)
        {
            // convert to milliseconds, rounding down
            osd_ticks_t msec = (duration * 1000 / osd_ticks_per_second());

            // only sleep if at least 2 full milliseconds
            if (msec >= 2)
            {
                //HANDLE current_thread = GetCurrentThread();
                //int old_priority = GetThreadPriority(current_thread);

                // take a couple of msecs off the top for good measure
                msec -= 2;

                // bump our thread priority super high so that we get
                // priority when we need it
                //SetThreadPriority(current_thread, THREAD_PRIORITY_TIME_CRITICAL);
                System.Threading.Thread.Sleep((int)msec);
                //SetThreadPriority(current_thread, old_priority);
            }
        }


        /***************************************************************************
            WORK ITEM INTERFACES
        ***************************************************************************/

        /*-----------------------------------------------------------------------------
            osd_work_queue_alloc: create a new work queue

            Parameters:

                flags - one or more of the WORK_QUEUE_FLAG_* values ORed together:

                    WORK_QUEUE_FLAG_IO - indicates that the work queue will do some
                        I/O; this may be a useful hint so that threads are created
                        even on single-processor systems since I/O can often be
                        overlapped with other work

                    WORK_QUEUE_FLAG_MULTI - indicates that the work queue should
                        take advantage of as many processors as it can; items queued
                        here are assumed to be fully independent or shared

                    WORK_QUEUE_FLAG_HIGH_FREQ - indicates that items are expected
                        to be queued at high frequency and acted upon quickly; in
                        general, this implies doing some spin-waiting internally
                        before falling back to OS-specific synchronization

            Return value:

                A pointer to an allocated osd_work_queue object.

            Notes:

                A work queue abstracts the notion of how potentially threaded work
                can be performed. If no threading support is available, it is a
                simple matter to execute the work items as they are queued.
        -----------------------------------------------------------------------------*/
        public override osd_work_queue osd_work_queue_alloc(int flags)
        {
            return null;
        }


        /*-----------------------------------------------------------------------------
            osd_work_queue_items: return the number of pending items in the queue

            Parameters:

                queue - pointer to an osd_work_queue that was previously created via
                    osd_work_queue_alloc

            Return value:

                The number of incomplete items remaining in the queue.
        -----------------------------------------------------------------------------*/
        //int osd_work_queue_items(osd_work_queue *queue);


        /*-----------------------------------------------------------------------------
            osd_work_queue_wait: wait for the queue to be empty

            Parameters:

                queue - pointer to an osd_work_queue that was previously created via
                    osd_work_queue_alloc

                timeout - a timeout value in osd_ticks_per_second()

            Return value:

                TRUE if the queue is empty; FALSE if the wait timed out before the
                queue was emptied.
        -----------------------------------------------------------------------------*/
        public override bool osd_work_queue_wait(osd_work_queue queue, osd_ticks_t timeout)
        {
            return true;
        }


        /*-----------------------------------------------------------------------------
            osd_work_queue_free: free a work queue, waiting for all items to complete

            Parameters:

                queue - pointer to an osd_work_queue that was previously created via
                    osd_work_queue_alloc

            Return value:

                None.
        -----------------------------------------------------------------------------*/
        public override void osd_work_queue_free(osd_work_queue queue)
        {
        }


        /*-----------------------------------------------------------------------------
            osd_work_item_queue_multiple: queue a set of work items

            Parameters:

                queue - pointer to an osd_work_queue that was previously created via
                    osd_work_queue_alloc

                callback - pointer to a function that will do the work

                numitems - number of work items to queue

                param - a void * parameter that can be used to pass data to the
                    function

                paramstep - the number of bytes to increment param by for each item
                    queued; for example, if you have an array of work_unit objects,
                    you can point param to the base of the array and set paramstep to
                    sizeof(work_unit)

                flags - one or more of the WORK_ITEM_FLAG_* values ORed together:

                    WORK_ITEM_FLAG_AUTO_RELEASE - indicates that the work item
                        should be automatically freed when it is complete

            Return value:

                A pointer to the final allocated osd_work_item in the list.

            Notes:

                On single-threaded systems, this function may actually execute the
                work item immediately before returning.
        -----------------------------------------------------------------------------*/
        public override osd_work_item osd_work_item_queue_multiple(osd_work_queue queue, osd_work_callback callback, int numitems, List<Object> parambase, /*int paramstep,*/ UInt32 flags)
        {
            return null;
        }


        /*-----------------------------------------------------------------------------
            osd_break_into_debugger: break into the hosting system's debugger if one
                is attached

            Parameters:

                message - pointer to string to output to the debugger

            Return value:

                None.

            Notes:

                This function is called when an assertion or other important error
                occurs. If a debugger is attached to the current process, it should
                break into the debugger and display the given message.
        -----------------------------------------------------------------------------*/
        public override void osd_break_into_debugger(string message)
        {
            osd_printf_error(message);
            //Debugger.Break();
            throw new Exception(string.Format("osd_break_into_debugger - {0}", message));
        }


        /***************************************************************************
            MIDI I/O INTERFACES
        ***************************************************************************/

        public abstract class osd_midi_device_WinForms : osd_midi_device
        {
        }


        public override void osd_list_network_adapters()
        {
        }


        /*-----------------------------------------------------------------------------
            osd_subst_env: substitute environment variables with values

            Parameters:

                dst - result pointer
                src - source string

        -----------------------------------------------------------------------------*/
        public override void osd_subst_env(out string dst, string src)
        {
            throw new emu_unimplemented();
        }
    }
}
