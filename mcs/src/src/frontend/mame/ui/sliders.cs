// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int32_t = System.Int32;
using uint32_t = System.UInt32;


namespace mame.ui
{
    class menu_sliders : menu
    {
        //enum
        //{
        //const unknown INPUT_GROUPS   = 0;
        //const unknown INPUT_SPECIFIC = 1;
        //}


        bool m_menuless_mode;
        bool m_hidden;


        public menu_sliders(mame_ui_manager mui, render_container container, bool menuless_mode = false)
            : base(mui, container)
        {
            m_hidden = menuless_mode;
            m_menuless_mode = menuless_mode;
        }


        /*-------------------------------------------------
            menu_sliders_populate - populate the sliders
            menu
        -------------------------------------------------*/
        protected override void populate(ref float customtop, ref float custombottom)
        {
            string tempstring;

            /* add UI sliders */
            std.vector<menu_item> ui_sliders = ui().get_slider_list();
            foreach (menu_item item in ui_sliders)
            {
                if (item.type == menu_item_type.SLIDER)
                {
                    slider_state slider = (slider_state)item.ref_;
                    int32_t curval = slider.update(machine(), slider.arg, slider.id, out tempstring, slider_state.SLIDER_NOCHANGE);
                    uint32_t flags = 0;
                    if (curval > slider.minval)
                        flags |= FLAG_LEFT_ARROW;
                    if (curval < slider.maxval)
                        flags |= FLAG_RIGHT_ARROW;
                    item_append(slider.description, tempstring, flags, slider, menu_item_type.SLIDER);
                }
                else
                {
                    item_append(item);
                }
            }

            item_append(menu_item_type.SEPARATOR);

            /* add OSD sliders */
            std.vector<menu_item> osd_sliders = machine().osd().get_slider_list();
            foreach (menu_item item in osd_sliders)
            {
                if (item.type == menu_item_type.SLIDER)
                {
                    slider_state slider = (slider_state)item.ref_;
                    int32_t curval = slider.update(machine(), slider.arg, slider.id, out tempstring, slider_state.SLIDER_NOCHANGE);
                    uint32_t flags = 0;
                    if (curval > slider.minval)
                        flags |= FLAG_LEFT_ARROW;
                    if (curval < slider.maxval)
                        flags |= FLAG_RIGHT_ARROW;
                    item_append(slider.description, tempstring, flags, slider);
                }
                else
                {
                    item_append(item);
                }
            }

            custombottom = 2.0f * ui().get_line_height() + 2.0f * UI_BOX_TB_BORDER;
        }


        /*-------------------------------------------------
            menu_sliders - handle the sliders menu
        -------------------------------------------------*/
        protected override void handle()
        {
            menu_event menu_event;

            /* process the menu */
            menu_event = process(PROCESS_LR_REPEAT | (m_hidden ? PROCESS_CUSTOM_ONLY : 0));
            if (menu_event != null)
            {
                /* handle keys if there is a valid item selected */
                if (menu_event.itemref != null && menu_event.type == menu_item_type.SLIDER)
                {
                    slider_state slider = (slider_state)menu_event.itemref;
                    string unused;
                    int curvalue = slider.update(machine(), slider.arg, slider.id, out unused, slider_state.SLIDER_NOCHANGE);
                    int increment = 0;
                    bool alt_pressed = machine().input().code_pressed(input_global.KEYCODE_LALT) || machine().input().code_pressed(input_global.KEYCODE_RALT);
                    bool ctrl_pressed = machine().input().code_pressed(input_global.KEYCODE_LCONTROL) || machine().input().code_pressed(input_global.KEYCODE_RCONTROL);
                    bool shift_pressed = machine().input().code_pressed(input_global.KEYCODE_LSHIFT) || machine().input().code_pressed(input_global.KEYCODE_RSHIFT);

                    switch (menu_event.iptkey)
                    {
                        /* toggle visibility */
                        case (int)ioport_type.IPT_UI_ON_SCREEN_DISPLAY:
                            if (m_menuless_mode)
                                stack_pop();
                            else
                                m_hidden = !m_hidden;
                            break;

                        /* decrease value */
                        case (int)ioport_type.IPT_UI_LEFT:
                            if (alt_pressed && shift_pressed)
                                increment = -1;
                            if (alt_pressed)
                                increment = -(curvalue - slider.minval);
                            else if (shift_pressed)
                                increment = (slider.incval > 10) ? -(slider.incval / 10) : -1;
                            else if (ctrl_pressed)
                                increment = -slider.incval * 10;
                            else
                                increment = -slider.incval;
                            break;

                        /* increase value */
                        case (int)ioport_type.IPT_UI_RIGHT:
                            if (alt_pressed && shift_pressed)
                                increment = 1;
                            if (alt_pressed)
                                increment = slider.maxval - curvalue;
                            else if (shift_pressed)
                                increment = (slider.incval > 10) ? (slider.incval / 10) : 1;
                            else if (ctrl_pressed)
                                increment = slider.incval * 10;
                            else
                                increment = slider.incval;
                            break;

                        /* restore default */
                        case (int)ioport_type.IPT_UI_SELECT:
                            increment = slider.defval - curvalue;
                            break;
                    }

                    /* handle any changes */
                    if (increment != 0)
                    {
                        int newvalue = curvalue + increment;

                        /* clamp within bounds */
                        if (newvalue < slider.minval)
                            newvalue = slider.minval;
                        if (newvalue > slider.maxval)
                            newvalue = slider.maxval;

                        /* update the slider and recompute the menu */
                        slider.update(machine(), slider.arg, slider.id, out unused, newvalue);
                        reset(reset_options.REMEMBER_REF);
                    }
                }

                /* if we are selecting an invalid item and we are hidden, skip to the next one */
                else if (m_hidden)
                {
                    /* if we got here via up or page up, select the previous item */
                    if (menu_event.iptkey == (int)ioport_type.IPT_UI_UP || menu_event.iptkey == (int)ioport_type.IPT_UI_PAGE_UP)
                    {
                        selected = (selected + item.size() - 1) % item.size();
                        validate_selection(-1);
                    }

                    /* otherwise select the next item */
                    else if (menu_event.iptkey == (int)ioport_type.IPT_UI_DOWN || menu_event.iptkey == (int)ioport_type.IPT_UI_PAGE_DOWN)
                    {
                        selected = (selected + 1) % item.size();
                        validate_selection(1);
                    }
                }
            }
        }


        /*-------------------------------------------------
            menu_sliders_custom_render - perform our special
            rendering
        -------------------------------------------------*/
        protected override void custom_render(object selectedref, float top, float bottom, float x1, float y1, float x2, float y2)  // void *selectedref
        {
            slider_state curslider = (slider_state)selectedref;
            if (curslider != null)
            {
                float bar_left;
                float bar_area_top;
                float bar_width;
                float bar_area_height;
                float bar_top;
                float bar_bottom;
                float default_x;
                float current_x;
                float line_height = mame_machine_manager.instance().ui().get_line_height();
                float percentage;
                float default_percentage;
                string tempstring;
                float text_height;
                int curval;

                /* determine the current value and text */
                curval = curslider.update(machine(), curslider.arg, curslider.id, out tempstring, slider_state.SLIDER_NOCHANGE);

                /* compute the current and default percentages */
                percentage = (float)(curval - curslider.minval) / (float)(curslider.maxval - curslider.minval);
                default_percentage = (float)(curslider.defval - curslider.minval) / (float)(curslider.maxval - curslider.minval);

                /* assemble the text */
                tempstring = string.Format("{0} ", curslider.description);  //.ins(0, " ").ins(0, curslider.description);

                /* move us to the bottom of the screen, and expand to full width */
                y2 = 1.0f - UI_BOX_TB_BORDER;
                y1 = y2 - bottom;
                x1 = UI_BOX_LR_BORDER;
                x2 = 1.0f - UI_BOX_LR_BORDER;

                /* draw extra menu area */
                ui().draw_outlined_box(container(), x1, y1, x2, y2, UI_BACKGROUND_COLOR);
                y1 += UI_BOX_TB_BORDER;

                /* determine the text height */
                float unused;
                ui().draw_text_full(container(), tempstring, 0, 0, x2 - x1 - 2.0f * UI_BOX_LR_BORDER,
                            text_layout.text_justify.CENTER, text_layout.word_wrapping.TRUNCATE, mame_ui_manager.draw_mode.NONE, rgb_t.white(), rgb_t.black(), out unused, out text_height);

                /* draw the thermometer */
                bar_left = x1 + UI_BOX_LR_BORDER;
                bar_area_top = y1;
                bar_width = x2 - x1 - 2.0f * UI_BOX_LR_BORDER;
                bar_area_height = line_height;

                /* compute positions */
                bar_top = bar_area_top + 0.125f * bar_area_height;
                bar_bottom = bar_area_top + 0.875f * bar_area_height;
                default_x = bar_left + bar_width * default_percentage;
                current_x = bar_left + bar_width * percentage;

                /* fill in the percentage */
                container().add_rect(bar_left, bar_top, current_x, bar_bottom, UI_SLIDER_COLOR, PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));

                /* draw the top and bottom lines */
                container().add_line(bar_left, bar_top, bar_left + bar_width, bar_top, UI_LINE_WIDTH, UI_BORDER_COLOR, PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));
                container().add_line(bar_left, bar_bottom, bar_left + bar_width, bar_bottom, UI_LINE_WIDTH, UI_BORDER_COLOR, PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));

                /* draw default marker */
                container().add_line(default_x, bar_area_top, default_x, bar_top, UI_LINE_WIDTH, UI_BORDER_COLOR, PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));
                container().add_line(default_x, bar_bottom, default_x, bar_area_top + bar_area_height, UI_LINE_WIDTH, UI_BORDER_COLOR, PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));

                /* draw the actual text */
                mame_machine_manager.instance().ui().draw_text_full(container(), tempstring, x1 + UI_BOX_LR_BORDER, y1 + line_height, x2 - x1 - 2.0f * UI_BOX_LR_BORDER,
                            text_layout.text_justify.CENTER, text_layout.word_wrapping.WORD, mame_ui_manager.draw_mode.NORMAL, UI_TEXT_COLOR, UI_TEXT_BG_COLOR, out unused, out text_height);
            }
        }


        /*-------------------------------------------------
         ui_slider_ui_handler - pushes the slider
         menu on the stack and hands off to the
         standard menu handler
         -------------------------------------------------*/
        public static new UInt32 ui_handler(render_container container, mame_ui_manager mui)
        {
            UInt32 result;

            throw new emu_unimplemented();
        }
    }
}
