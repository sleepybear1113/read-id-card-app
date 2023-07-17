using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Win32;
using ReadIdCardApp.db;
using ReadIdCardApp.excel;

namespace ReadIdCardApp;

public partial class DataImportWindow : Window {
    private MainWindow MainWindow;

    class ExcelImportResult {
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public int UpdateCount { get; set; }
        public List<string> ErrMsgList = new();
    }

    public DataImportWindow() {
        InitializeComponent();
    }

    public DataImportWindow(Schedule? schedule, MainWindow mainWindow) {
        InitializeComponent();
        MainWindow = mainWindow;
        Init(schedule);
    }

    private void Init(Schedule? schedule) {
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        ComboBoxScheduleList.ItemsSource = new ObservableCollection<Schedule>(MainWindow.Schedules);
        if (schedule != null) {
            ComboBoxScheduleList.SelectedItem = schedule;
        }
    }

    private void KeyDownAction(object sender, KeyEventArgs e) {
        if (e.Key == Key.Escape) {
            Close();
        }
    }

    private void Checked(object sender, RoutedEventArgs e) {
        if (CheckBoxUseExcelScheduleId.IsChecked == null || CheckBoxUseExcelScheduleId.IsChecked == false) {
            ComboBoxScheduleList.IsEnabled = true;
        } else {
            ComboBoxScheduleList.IsEnabled = false;
            ComboBoxScheduleList.SelectedItem = null;
        }
    }

    private void ImportAppend(object sender, RoutedEventArgs e) {
        var schedule = ComboBoxScheduleList.SelectedItem as Schedule;
        var scheduleIdSet = new HashSet<int>();
        foreach (var s in MainWindow.Schedules) {
            scheduleIdSet.Add((int)s.Id);
        }

        var excelImportResult =
            ImportFromExcel(schedule?.Id, CheckBoxOnlyImportValidIdCard.IsChecked == true, scheduleIdSet);


        MainWindow.LoadPeopleList();
        MainWindow.LoadSchedules();
        if (excelImportResult == null) {
            return;
        }

        string msg =
            $"导入结束！成功新增{excelImportResult.SuccessCount}条，成功更新{excelImportResult.UpdateCount}条。失败{excelImportResult.FailCount}条。";
        var messageBoxButton = MessageBoxButton.OK;
        if (excelImportResult.FailCount > 0) {
            msg += "\n>>点击“确定”查看失败信息<<";
            messageBoxButton = MessageBoxButton.YesNo;
        }

        MessageBoxResult result = MessageBox.Show(msg, "导入提示", messageBoxButton, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes && excelImportResult.FailCount > 0) {
            result = MessageBox.Show(string.Join("\n", excelImportResult.ErrMsgList), "导入提示", MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            
        }

        Close();
    }

    private void ImportClear(object sender, RoutedEventArgs e) {
        var selectedFileName = SelectExcelFile();
    }

    private ExcelImportResult?
        ImportFromExcel(int? scheduleId, bool onlyImportValidIdCard, HashSet<int> scheduleIdSet) {
        var peopleList = ImportFromExcelToPeopleList(scheduleId, onlyImportValidIdCard, scheduleIdSet);
        if (peopleList == null) {
            return null;
        }

        ExcelImportResult excelImportResult = new ExcelImportResult();
        if (peopleList.Count == 0) {
            return excelImportResult;
        }

        HashSet<int?> remainedScheduleIdSet = new HashSet<int?>();
        Dictionary<string, int> scheduleNameMap = new Dictionary<string, int>();
        foreach (var people in peopleList) {
            if (!string.IsNullOrEmpty(people.ErrMsg)) {
                continue;
            }

            if (people.ScheduleId != null) {
                remainedScheduleIdSet.Add(people.ScheduleId);
            }

            if (people.ScheduleId == null || people.ScheduleId == 0) {
                scheduleNameMap[people.ScheduleName] = scheduleNameMap.Count;
            }
        }

        string[] scheduleNameList = new string[scheduleNameMap.Count];
        foreach (var keyValuePair in scheduleNameMap) {
            scheduleNameList[keyValuePair.Value] = keyValuePair.Key;
        }

        foreach (var scheduleName in scheduleNameList) {
            var id = new Schedule() { Name = scheduleName }.InsertToDb();
            scheduleNameMap[scheduleName] = id;
        }

        foreach (var people in peopleList) {
            if (!string.IsNullOrEmpty(people.ErrMsg) || people.ScheduleId > 0) {
                continue;
            }

            people.ScheduleId = scheduleNameMap[people.ScheduleName];
        }

        var existPeopleList = new People().SelectByScheduleIds(remainedScheduleIdSet.ToList());
        Dictionary<string, int?> scheduleIdIdCardKeyMap = new Dictionary<string, int?>();
        foreach (var people in existPeopleList) {
            string key = $"{people.ScheduleId}-{people.IdCard}";
            scheduleIdIdCardKeyMap.Add(key, people.Id);
        }

        foreach (var people in peopleList) {
            if (!string.IsNullOrEmpty(people.ErrMsg)) {
                excelImportResult.FailCount++;
                excelImportResult.ErrMsgList.Add($"{people.Id + 1}@{people.ErrMsg}");
                continue;
            }

            string key = $"{people.ScheduleId}-{people.IdCard}";
            if (scheduleIdIdCardKeyMap.TryGetValue(key, out var id)) {
                if (id != null) {
                    people.Id = id;
                    people.UpdateDb();
                    excelImportResult.UpdateCount++;
                }
            } else {
                people.InsertToDb();
                excelImportResult.SuccessCount++;
            }
        }

        return excelImportResult;
    }

    private List<People>? ImportFromExcelToPeopleList(int? scheduleId, bool onlyImportValidIdCard,
        HashSet<int> scheduleIdSet) {
        var selectedFileName = SelectExcelFile();
        if (selectedFileName == null) {
            return null;
        }

        var excelData = ReadExcel.Read(selectedFileName);
        if (excelData == null) {
            return new List<People>();
        }

        var peoples = People.BuildFromExcelDataList(excelData);
        if (peoples == null) {
            return new List<People>();
        }

        HashSet<string> idCardSet = new HashSet<string>();
        for (var i = 0; i < peoples.Count; i++) {
            var people = peoples[i];
            people.Id = i;

            if (scheduleId != null) {
                people.ScheduleId = scheduleId;
            } else {
                int? pScheduleId = people.ScheduleId;
                if (pScheduleId != null && pScheduleId != 0 && !scheduleIdSet.Contains((int)pScheduleId)) {
                    people.ErrMsg += $"场次id[{pScheduleId}]在已有场次中不存在;";
                }
            }

            // 如果没有 error 那么判断身份证是否重复
            if (string.IsNullOrEmpty(people.ErrMsg)) {
                if (idCardSet.Contains(people.IdCard)) {
                    people.ErrMsg += "同一Excel中身份证重复不予导入;";
                }
            }

            if (onlyImportValidIdCard && !string.IsNullOrEmpty(people.IdCardValid)) {
                people.ErrMsg += people.IdCardValid;
            }

            // 没有 error 后将合法的身份证加入 set
            if (string.IsNullOrEmpty(people.ErrMsg)) {
                idCardSet.Add(people.IdCard);
            }
        }

        return peoples;
    }

    public string? SelectExcelFile() {
        OpenFileDialog openFileDialog = new OpenFileDialog {
            Filter = "Excel文件(*.xls;*.xlsx)|*.xls;*.xlsx",
            Title = "选择Excel导入文件",
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() == true) {
            return openFileDialog.FileName;
        }

        return null;
    }
}