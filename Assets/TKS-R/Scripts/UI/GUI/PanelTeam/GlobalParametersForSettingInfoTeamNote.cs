using I2.Loc;

namespace TKSR
{
    public class GlobalParametersForSettingInfoTeamNote : RegisterGlobalParameters
	{
		public override string GetParameterValue(string ParamName)
        {
            var curDocument = DocumentDataManager.Instance.GetCurrentDocument();
            if (ParamName == ResourceUtils.I2PARAM_MAINPLAYER_LASTNAME)
                return curDocument.LastName;            // For your game, get this value from your Game Manager
            
            if (ParamName == ResourceUtils.I2PARAM_MAINPLAYER_FIRSTNAME)
                return curDocument.FirstName;

            return null;
        }

	}
}