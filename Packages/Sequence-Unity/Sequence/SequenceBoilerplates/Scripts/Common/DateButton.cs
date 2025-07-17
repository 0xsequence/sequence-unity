using TMPro;
using UnityEngine;

namespace Sequence.Boilerplates
{
    public class DateButton : MonoBehaviour
    {
        public TextMeshProUGUI dayText;

        private int day;
        private int month;
        private int year;

        public void Setup(int day, int month, int year)
        {
            this.day = day;
            this.month = month;
            this.year = year;
            dayText.text = day.ToString();
        }

        public void OnDateSelected()
        {
            FindObjectOfType<DatePicker>().SetDate(day, month, year);
        }
    }
}
 