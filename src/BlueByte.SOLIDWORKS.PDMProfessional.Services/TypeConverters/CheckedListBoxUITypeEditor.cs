/// <summary>
/// 	''' Custom, editable checked listbox control
/// 	''' </summary>
/// 	''' <remarks>This demo loads a comma-delimited string collection of Urls and boolean values to set the checked state for each item.</remarks>
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.VisualBasic;

namespace BlueByte.SOLIDWORKS.PDMProfessional.Services.TypeConverters
{

    public class CheckedListBoxUITypeEditor : UITypeEditor
    {

        public static string[] ItemsSource { get; set; }

        private IWindowsFormsEditorService editorService;

        private CheckedListBox checklistBox;

        // Indicates that this editor supports a modal form-based drop-down.
        public override bool IsDropDownResizable => true;

        // Indicates that the UITypeEditor provides a form-based interface.
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.DropDown;

        // Shows the drop-down control and sets the value based on user selection.
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (editorService != null)
                {
                    checklistBox = new CheckedListBox();
                    checklistBox.BorderStyle = BorderStyle.None;


                    if (ItemsSource != null)
                        checklistBox.Items.AddRange(ItemsSource);

                    var selectedValue = context.PropertyDescriptor.GetValue(context.Instance);
                    if (selectedValue != null)
                    {
                        var arr = selectedValue as string[];
                        if (arr != null)
                        {
                            foreach (var item in arr)
                            {
                                var index = checklistBox.Items.IndexOf(item);
                                if (index >= 0)
                                    checklistBox.SetItemChecked(index, true);
                            }
                        }
                    }


                    // Set the checked items based on the current property value
                    var selectedItems = (value as string)?.Split(',');
                    for (int i = 0; i < checklistBox.Items.Count; i++)
                    {
                        if (selectedItems != null && selectedItems.Contains(checklistBox.Items[i].ToString()))
                        {
                            checklistBox.SetItemChecked(i, true);
                        }
                    }


                    // Show the checklistbox dropdown
                    editorService.DropDownControl(checklistBox);

                    // Build the result string based on the checked items
                    var result = checklistBox.CheckedItems.Cast<string>().ToArray();

                    checklistBox.Dispose();
                    checklistBox = null;

                    return result;
                }
            }

            return value;
        }


    }
}
