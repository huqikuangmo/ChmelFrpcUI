using FrpcUI.Class;

namespace FrpcUI.Pages
{
    public class CompositeViewModel
    {
        public PeizhiwenjianViewModel PeizhiModel { get; set; }
        public SuidaoViewModel SuidaoModel { get; set; }
        public LogingModelViewModel LogingModels { get; set; }


        public CompositeViewModel()
        {
            PeizhiModel = new PeizhiwenjianViewModel();

            SuidaoModel = new SuidaoViewModel();

            LogingModels = new LogingModelViewModel();
        }
    }


}

