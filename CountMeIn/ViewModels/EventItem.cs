using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CountMeIn.ViewModels
{
    public class EventItem
    {

        public int ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "Required")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "Required")]
        public string Location { get; set; }

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "Required")]
        [DataType(DataType.Date, ErrorMessage = "Невалиден тип на данните")]
        [DisplayFormat(DataFormatString = "dd.MM.yyyy", ApplyFormatInEditMode = true)]
        //[Range(typeof(System.DateTime), "01.01.2014", "12.12.2099", ErrorMessage = "Въведете валиден период от време във формат дд.мм.гггг")]
        //[Range(typeof(string), "01.01.2014", "12.12.2099", ErrorMessage = "Въведете валиден период от време във формат дд.мм.гггг")]
        [RegularExpression(Configuration.ConfigurationSettings.DateRegex, ErrorMessage = "Въведете дата във формат дд.мм.гггг")]
        //[RegularExpression("(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[1,3-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})",ErrorMessage="xxxxxxxxxxx")]
        public string StartDate { get; set; }

        //public System.DateTime StartDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "Required")]
        [DataType(DataType.Date, ErrorMessage = "Невалиден тип на данните")]
        [DisplayFormat(DataFormatString = "dd.MM.yyyy", ApplyFormatInEditMode = true)]
        //[Range(typeof(System.DateTime), "01.01.2014", "12.12.2099", ErrorMessage = "Въведете валиден период от време във формат дд.мм.гггг")]
        //[Range(typeof(System.String), "01.01.2014", "12.12.2099", ErrorMessage = "Въведете валиден период от време във формат дд.мм.гггг")]
        [RegularExpression(Configuration.ConfigurationSettings.DateRegex, ErrorMessage = "Въведете дата във формат дд.мм.гггг")]
        public string EndDate { get; set; }
        //public System.DateTime EndDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "Required")]
        [DataType(DataType.Time, ErrorMessage = "Невалиден тип на данните")]
        [DisplayFormat(DataFormatString = "HH:mm", ApplyFormatInEditMode = true)]
        //[RegularExpression("",ErrorMessage="koko")]
        //public string StartHour { get; set; }
        //[Range(typeof(System.DateTime), "00:00", "23:59", ErrorMessage = "Въведете валиден период от време във формат чч:мм")]
        //[Range(typeof(System.String), "00:00", "23:59", ErrorMessage = "Въведете валиден период от време във формат чч:мм")]
        [RegularExpression(@"^(?:[0-1][0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Въведете час във формат чч:мм")]
        public string StartHour { get; set; }
        //public System.DateTime StartHour { get; set; }

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "Required")]
        [DataType(DataType.Time, ErrorMessage = "Невалиден тип на данните")]
        [DisplayFormat(DataFormatString = "HH:mm", ApplyFormatInEditMode = true)]
        //[Range(typeof(System.DateTime), "00:00", "23:59", ErrorMessage = "Въведете валиден период от време във формат чч:мм")]
        //[Range(typeof(System.String), "00:00", "23:59", ErrorMessage = "Въведете валиден период от време във формат чч:мм")]
        [RegularExpression(@"^(?:[0-1][0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Въведете час във формат чч:мм")]
        public string EndHour { get; set; }
        //public System.DateTime EndHour { get; set; }

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "Required")]
        [Range(0, 22, ErrorMessageResourceType = typeof(App_GlobalResources.Common), ErrorMessageResourceName = "Number")]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public int HoursOffset { get; set; }


        [Range(0,22,ErrorMessageResourceType=typeof(App_GlobalResources.Common), ErrorMessageResourceName="NumberUser")]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public Nullable<int> MaxUsers { get; set; }

        public List<UserItem> Users { get; set; }

        public int OwnerID { get; set; }

        public int GroupID { get; set; }

        public bool IsPublic { get; set; }

        public int UsersCount
        {
            get
            {
                if (Users == null)
                    return 0;
                return Users.Count();
            }
        }

    }
}