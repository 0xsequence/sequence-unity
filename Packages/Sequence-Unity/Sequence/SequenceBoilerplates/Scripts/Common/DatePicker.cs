using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public class DatePicker : MonoBehaviour
    {
        public TextMeshProUGUI dateDisplay;
        public TMP_Dropdown yearDropdown;
        public TMP_Dropdown monthDropdown;
        public Transform gridContainer;
        public GameObject dateButtonPrefab, spacingButton; // Prefab for the date buttons

        private List<GameObject> dateButtons = new List<GameObject>();

        public DateTime Date{get;private set;}
        private void Start()
        {
            PopulateYearDropdown();
            PopulateMonthDropdown();

            yearDropdown.onValueChanged.AddListener(delegate { GenerateCalendar(); });
            monthDropdown.onValueChanged.AddListener(delegate { GenerateCalendar(); });

            GenerateCalendar(); // Generate calendar on start
        }

        public void SetDate(int day, int month, int year)
        {
            Date = new DateTime(year,month, day);
            dateDisplay.text = Date.ToString();
            transform.parent.gameObject.SetActive(false);

        }

        private void PopulateYearDropdown()
        {
            yearDropdown.ClearOptions();
            List<string> years = new List<string>();
            int currentYear = DateTime.Now.Year;

            for (int i = currentYear; i <= currentYear + 50; i++)
            {
                years.Add(i.ToString());
            }

            yearDropdown.AddOptions(years);
            yearDropdown.value = 0; // Set current year as default
        }

        private void PopulateMonthDropdown()
        {
            monthDropdown.ClearOptions();
            List<string> months = new List<string>
        {
            "Jan", "Feb", "Mar", "Apr", "May", "Jun",
            "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        };

            monthDropdown.AddOptions(months);
            monthDropdown.value = DateTime.Now.Month - 1; // Set current month as default
        }

        private void GenerateCalendar()
        {
            // Clear previous buttons
            foreach (var button in dateButtons)
            {
                Destroy(button);
            }
            dateButtons.Clear();

            int selectedYear = int.Parse(yearDropdown.options[yearDropdown.value].text);
            int selectedMonth = monthDropdown.value + 1;
            DateTime firstDayOfMonth = new DateTime(selectedYear, selectedMonth, 1);
            int daysInMonth = DateTime.DaysInMonth(selectedYear, selectedMonth);
            int startDayIndex = (int)firstDayOfMonth.DayOfWeek; // Sunday = 0, Monday = 1, etc.

            // Adjust start day index to align Monday as the first column
            startDayIndex = (startDayIndex == 0) ? 6 : startDayIndex - 1;

            // Instantiate empty slots for alignment
            for (int i = 0; i < startDayIndex; i++)
            {
                GameObject emptyButton = Instantiate(spacingButton, gridContainer);
                dateButtons.Add(emptyButton);

            }

            // Instantiate date buttons
            for (int day = 1; day <= daysInMonth; day++)
            {
                GameObject dateButton = Instantiate(dateButtonPrefab, gridContainer);
                dateButton.GetComponent<DateButton>().Setup(day, selectedMonth, selectedYear);
                dateButtons.Add(dateButton);
            }
        }
    }

}
