using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVRControl.Commands;
using AVRLibrary;


namespace AVRControl
{
    public class MainViewModel
    {


        private AVRConnection _connection = null;

        private RelayCommand _scanAction;

        public RelayCommand ScanAction
        {
            get { return _scanAction; }
            set { _scanAction = value; }
        }


        public MainViewModel()
        {
            _connection = new AVRConnection();
            _scanAction = new RelayCommand(x => { _connection.StartScanning(); });
        }

    }
}
