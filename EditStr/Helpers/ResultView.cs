using System.Text;

namespace EjoProduction.Helpers
{
    public class ResultView
    {

      
        public string JSOutput { get; }
        public bool IsSuccess => false;

        private ResultView(string Message , bool IsSuccess)
        {

            StringBuilder script = new StringBuilder();
          
            script.Append(" $.toast({");
            script.Append(@"text:' " + System.Web.HttpUtility.HtmlEncode(Message) + "',");

            if (!IsSuccess)
            {
                script.Append(@" heading:'Error',");
                script.Append(" position: 'top-center', loaderBg: '#ff6849' ,  hideAfter: 3500 , icon: 'error' , stack: 6});");

            }
            else
            {
                script.Append(@" heading:'Successful',");
                script.Append(" position: 'top-center', loaderBg: '#ff6849' ,  hideAfter: 3500 , icon: 'success' , stack: 6});");

            }
            JSOutput = script.ToString();
        }
       
        public static ResultView Success(string Message) => new ResultView(Message, true);
        public static ResultView Failure(string Message) => new ResultView(Message, false);
    }
}
