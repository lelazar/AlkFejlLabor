using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TaskManager
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimePickerPopupPage : PopupPage
    {
        public TaskCompletionSource<TimeSpan> DisappearingTask { get; private set; }  // https://stackoverflow.com/questions/5095183/how-to-return-a-value-from-a-popup-page-in-wpf
        public TimeSpan SelectedTime { get; private set; }

        public TimePickerPopupPage()
        {
            InitializeComponent();
            DisappearingTask = new TaskCompletionSource<TimeSpan>();  // Create a new task completion source
        }

        private void OnConfirmClicked(object sender, EventArgs e)
        {
            if (!DisappearingTask.Task.IsCompleted)
            {
                SelectedTime = timePicker.Time;
                DisappearingTask.SetResult(SelectedTime);  // Set the result of the task completion source to the selected date
            }
            PopupNavigation.Instance.PopAsync();  // Pop the popup page off the navigation stack. This will trigger the OnDisappearing event
        }
    }
}