using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace TaskManager
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DatePickerPopupPage : PopupPage
    {
        public TaskCompletionSource<DateTime> DisappearingTask { get; private set; }  // https://stackoverflow.com/questions/5095183/how-to-return-a-value-from-a-popup-page-in-wpf
        public DateTime SelectedDate { get; private set; }

        public DatePickerPopupPage()
        {
            InitializeComponent();
            DisappearingTask = new TaskCompletionSource<DateTime>();  // Create a new task completion source
        }

        private void OnConfirmClicked(object sender, EventArgs e)
        {
            if (!DisappearingTask.Task.IsCompleted)
            {
                SelectedDate = datePicker.Date;
                DisappearingTask.SetResult(SelectedDate);  // Set the result of the task completion source to the selected date
            }
            PopupNavigation.Instance.PopAsync();  // Pop the popup page off the navigation stack. This will trigger the OnDisappearing event
        }
    }
}