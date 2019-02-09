
namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
	/// <summary>
	/// Summary description for WizardControlInterface.
	/// </summary>
	internal interface IWizardControl
	{
		bool NextButtonEnabled
		{
			get;
		}
		bool NeedSummary
		{
			get;
		}
	}
}
