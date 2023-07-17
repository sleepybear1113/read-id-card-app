using System;
using System.Windows;
using System.Windows.Input;
using ReadIdCardApp.db;
using ReadIdCardApp.utils;

namespace ReadIdCardApp;

public partial class ScheduleModifyWindow : Window {
    public Schedule Schedule;

    public ScheduleModifyWindow() {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
    }

    public ScheduleModifyWindow(Schedule? schedule) {
        schedule ??= new Schedule();

        Schedule = schedule;
        InitializeComponent();
        Title = Schedule.Id == null ? "新增签到场次" : "修改签到场次";
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        DataContext = Schedule;
        TextBoxModifyScheduleName.Focus();

        InitDataPicker(Schedule);
    }

    private void InitDataPicker(Schedule schedule) {
        DatePickerScheduleStartTime.SelectedDate = Util.FormatFromUnixTime(schedule.TimeBegin);
        DatePickerScheduleEndTime.SelectedDate = Util.FormatFromUnixTime(schedule.TimeEnd);
        TimePickerScheduleStartTime.Value = Util.FormatFromUnixTime(schedule.TimeBegin);
        TimePickerScheduleEndTime.Value = Util.FormatFromUnixTime(schedule.TimeEnd);
    }

    private void Modify(object sender, RoutedEventArgs e) {
        Modify();
    }

    private long? CombineToUnixTime(DateTime? date, DateTime? time, int dayAdd, int msAdd) {
        if (date.HasValue) {
            if (time.HasValue) {
                return Util.ToUnixMillisecond(date.Value.Date + time.Value.TimeOfDay);
            } else {
                return Util.ToUnixMillisecond(date.Value.Date.AddDays(dayAdd).AddMilliseconds(msAdd));
            }
        } else {
            return null;
        }
    }

    private void Modify() {
        Schedule.Name = Schedule.Name.Trim();
        if (string.IsNullOrEmpty(Schedule.Name) || string.IsNullOrWhiteSpace(Schedule.Name)) {
            MessageBox.Show("场次名不能为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            TextBoxModifyScheduleName.Focus();
            return;
        }

        Schedule.TimeBegin =
            CombineToUnixTime(DatePickerScheduleStartTime.SelectedDate, TimePickerScheduleStartTime.Value, 0, 0);
        Schedule.TimeEnd =
            CombineToUnixTime(DatePickerScheduleEndTime.SelectedDate, TimePickerScheduleEndTime.Value, 1, -1);
        if (Schedule.Id == null) {
            var id = Schedule.InsertToDb();
            Schedule.Id = id;
            MainWindow.Schedules.Add(Schedule);
        } else {
            for (var i = 0; i < MainWindow.Schedules.Count; i++) {
                if (MainWindow.Schedules[i].Id == Schedule.Id) {
                    Schedule.UpdateDb();
                    MainWindow.Schedules[i] = Schedule;
                }
            }
        }

        Close();
    }

    private void KeyDownAction(object sender, KeyEventArgs e) {
        if (e.Key == Key.Escape) {
            Close();
        } else if (e.Key == Key.Enter) {
            ButtonModifyScheduleConfirm.Focus();
            Modify();
        }
    }

    private void ClearTimeBegin(object sender, RoutedEventArgs e) {
        DatePickerScheduleStartTime.SelectedDate = Util.FormatFromUnixTime(null);
        TimePickerScheduleStartTime.Value = Util.FormatFromUnixTime(null);
    }

    private void ClearTimeEnd(object sender, RoutedEventArgs e) {
        DatePickerScheduleEndTime.SelectedDate = Util.FormatFromUnixTime(null);
        TimePickerScheduleEndTime.Value = Util.FormatFromUnixTime(null);
    }
}