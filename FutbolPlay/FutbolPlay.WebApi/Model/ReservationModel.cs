using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutbolPlay.WebApi.Model
{
    public class ReservationModel : INotifyPropertyChanged
    {
        public int IdReservation { get; set; }
        public PlaceModel Place { get; set; }
        public PitchModel Pitch { get; set; }
        public DateTime Date { get; set; }

        decimal _price;
        public decimal Price { get { return _price; } set { _price = value; OnPropertyChanged("Price"); } }

        string _description;
        public string Description { get { return _description; } set { _description = value; OnPropertyChanged("Description"); } }

        ReservationStatus _status;
        public ReservationStatus Status { get { return _status; } set { _status = value; OnPropertyChanged("Status"); } }
        public UserModel User { get; set; }
        public int Source { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum ReservationStatus
    {
        None = 0, Pending = 1, Ok = 2, CancelUser = 3, CancelPlace= 4, Running = 5, Close = 6, Conflict = 7, Error = 8
    }
}
