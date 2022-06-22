using Samba.Domain.Models.Accounts;
using Samba.Domain.Models.Tickets;
using Samba.Infrastructure.Settings;
using Samba.Localization.Properties;
using Samba.Presentation.Common;

namespace Samba.Presentation.ViewModels
{
    public class ServiceViewModel : ObservableObject
    {
        public Calculation Model { get; set; }

        public ServiceViewModel(Calculation model)
        {
            Model = model;
        }

        public string Name { get { return Model.Name + " " + Description; } }

        public string Description
        {
            get
            {
                return Model.CalculationType == 0 || Model.CalculationType == 1 ? (Model.Amount / 100).ToString("#,#0.##%") : "";
            }
        }

        public string Amount
        {
            get
            {
                return (Model.CalculationAmount).ToString(LocalSettings.ReportCurrencyFormat);
            }
        }

        public decimal CalculationAmount { get { return Model.CalculationAmount; } }

        public void Refresh()
        {
            RaisePropertyChanged(() => Amount);
            RaisePropertyChanged(() => Description);
        }
    }
}
