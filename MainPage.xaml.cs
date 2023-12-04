using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Rg.Plugins.Popup;
using Rg.Plugins.Popup.Services;

namespace TaskManager
{
    public class TaskItem
    {
        public string TaskName { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime? ReminderTime { get; set; }
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High
    }

    public partial class MainPage : ContentPage
    {
        // https://learn.microsoft.com/en-us/dotnet/api/system.collections.objectmodel.observablecollection-1?view=net-8.0
        ObservableCollection<TaskItem> tasks = new ObservableCollection<TaskItem>();  // Observable collection is used to update the UI when the collection changes
        private System.Timers.Timer pomodoroTimer;
        private int pomodoroDuration = 25 * 60;  // 25 minutes in seconds
        private int timeLeft;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            lstTasks.ItemsSource = tasks;  // Set the list view's item source to the observable collection

            // Pomodoro timer setup
            pomodoroTimer = new System.Timers.Timer(1000);  // Create a new timer that ticks every second
            pomodoroTimer.Elapsed += OnPomodoroTimerElapsed;  // Subscribe to the timer's elapsed event
        }

        #region Pomodoro Timer Commands
        private void StartPomodoroTimer(object sender, EventArgs e)
        {
            timeLeft = pomodoroDuration;
            pomodoroTimer.Start();
        }

        private void PausePomodoroTimer(object sender, EventArgs e)
        {
            pomodoroTimer.Stop();
        }

        private void ResetPomodoroTimer(object sender, EventArgs e)
        {
            pomodoroTimer.Stop();
            timeLeft = pomodoroDuration;
            UpdateTimerLabel();
        }

        private void OnPomodoroTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timeLeft--;
            UpdateTimerLabel();

            if (timeLeft <= 0)
            {
                pomodoroTimer.Stop();
                Device.BeginInvokeOnMainThread(() => DisplayAlert("Pomodoro Complete", "Time for a break!", "OK"));
            }
        }

        private void UpdateTimerLabel()
        {
            var minutes = timeLeft / 60;  // Divide by 60 to get minutes
            var seconds = timeLeft % 60;  // Modulo 60 to get seconds
            string text = $"{minutes:00}:{seconds:00}";
            Device.BeginInvokeOnMainThread(() => pomodoroLabel.Text = text);  // Update the label on the UI thread
        }
        #endregion

        #region Task related button commands
        private void btnAddTask_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(taskEntry.Text))
            {
                tasks.Add(new TaskItem { TaskName = taskEntry.Text });
                taskEntry.Text = string.Empty;  // Clear the entry after adding a task
            }
        }

        private void DeleteTask_Clicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var taskToDelete = (TaskItem)button.CommandParameter;

            if (tasks.Contains(taskToDelete))
            {
                tasks.Remove(taskToDelete);
            }
        }

        private void SortTasks_Clicked(object sender, EventArgs e)
        {
            var sortedTasks = tasks.OrderByDescending(task => task.Priority).ToList();

            tasks.Clear();
            foreach (var task in sortedTasks)
            {
                tasks.Add(task);
            }
        }

        private void IncreasePrio_Clicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var task = (TaskItem)button.CommandParameter;

            if (task.Priority < TaskPriority.High)
            {
                task.Priority++;
                UpdateListView();
            }
        }

        private void DecreasePrio_Clicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var task = (TaskItem)button.CommandParameter;

            if (task.Priority > TaskPriority.Low)
            {
                task.Priority--;
                UpdateListView();
            }
        }

        private void UpdateListView()
        {
            var currentIndex = lstTasks.ItemsSource;  // Store the current item source
            lstTasks.ItemsSource = null;  // Set the item source to null to force the list view to update
            lstTasks.ItemsSource = currentIndex;  // Set the item source back to the original value
        }
        #endregion

        #region Task Reminder Commands
        private async void SetReminder_Clicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var task = (TaskItem)button.CommandParameter;

            // Display DatePicker and TimePicker dialog here
            var reminderDate = await DisplayDatePickerDialog();
            await Task.Delay(500);  // Wait for the date picker to disappear before displaying the time picker
            var reminderTime = await DisplayTimePickerDialog();

            task.ReminderTime = reminderDate + reminderTime;
            
            // TODO: Handle setting and storing the reminder

            // TODO: Handle displaying the reminder
        }

        private async Task<DateTime> DisplayDatePickerDialog()
        {
            var datePickerPopupPage = new DatePickerPopupPage();  // Create a new instance of the popup page
            await PopupNavigation.Instance.PushAsync(datePickerPopupPage);  // Push the page onto the navigation stack
            await datePickerPopupPage.DisappearingTask.Task;  // Wait for the page to disappear

            return datePickerPopupPage.SelectedDate;  // Return the selected date
        }

        private async Task<TimeSpan> DisplayTimePickerDialog()
        {
            var timePickerPopupPage = new TimePickerPopupPage();  // Create a new instance of the popup page
            await PopupNavigation.Instance.PushAsync(timePickerPopupPage);  // Push the page onto the navigation stack
            await timePickerPopupPage.DisappearingTask.Task;  // Wait for the page to disappear

            return timePickerPopupPage.SelectedTime;  // Return the selected time
        }
        #endregion

        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
                lstTasks.ItemsSource = tasks;  // If the search bar is empty, show all tasks
            else
                lstTasks.ItemsSource = tasks.Where(task => task.TaskName.ToLower().Contains(e.NewTextValue.ToLower()));  // Otherwise, show only tasks that contain the search text
        }
    }
}
