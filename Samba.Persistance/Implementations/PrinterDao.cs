using System.Collections.Generic;
using System.ComponentModel.Composition;
using Samba.Domain.Models.Accounts;
using Samba.Domain.Models.Settings;
using Samba.Infrastructure.Data;
using Samba.Infrastructure.Data.Validation;
using Samba.Localization.Properties;
using Samba.Persistance.Data;

namespace Samba.Persistance.Implementations
{
    [Export(typeof(IPrinterDao))]
    class PrinterDao : IPrinterDao
    {
        [ImportingConstructor]
        public PrinterDao()
        {
            ValidatorRegistry.RegisterDeleteValidator(new PrinterDeleteValidator());
            ValidatorRegistry.RegisterDeleteValidator(new PrinterTemplateDeleteValidator());
        }

        public IEnumerable<Printer> GetPrinters()
        {
            return Dao.Query<Printer>();
        }

        public IEnumerable<PrinterTemplate> GetPrinterTemplates()
        {
            return Dao.Query<PrinterTemplate>();
        }
    }

    public class PrinterDeleteValidator : SpecificationValidator<Printer>
    {
        public override string GetErrorMessage(Printer model)
        {
            return Dao.Exists<Terminal>(x => x.ReportPrinterId == model.Id)
                ? string.Format(Resources.DeleteErrorUsedBy_f, Resources.Printer, Resources.Terminal)
                : Dao.Exists<Terminal>(x => x.TransactionPrinterId == model.Id)
                ? string.Format(Resources.DeleteErrorUsedBy_f, Resources.Printer, Resources.Terminal)
                : Dao.Exists<PrinterMap>(x => x.PrinterId == model.Id)
                ? string.Format(Resources.DeleteErrorUsedBy_f, Resources.Printer, Resources.PrintJob)
                : "";
        }
    }

    public class PrinterTemplateDeleteValidator : SpecificationValidator<PrinterTemplate>
    {
        public override string GetErrorMessage(PrinterTemplate model)
        {
            return Dao.Exists<PrinterMap>(y => y.PrinterTemplateId == model.Id)
                ? string.Format(Resources.DeleteErrorUsedBy_f, Resources.PrinterTemplate, Resources.PrintJob)
                : Dao.Exists<AccountTransactionDocumentType>(y => y.PrinterTemplateId == model.Id)
                ? string.Format(Resources.DeleteErrorUsedBy_f, Resources.PrinterTemplate, Resources.AccountTransactionDocument)
                : "";
        }
    }
}
