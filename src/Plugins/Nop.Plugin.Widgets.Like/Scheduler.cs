using Nop.Plugin.Widgets.Like.Service;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Like
{
   public class Scheduler : ITask
    {
        private readonly ILogger _logger;
        private readonly ILikeService _likeService;
        public Scheduler(ILogger logger, ILikeService likeService)
        {
            this._logger = logger;
            this._likeService = likeService;
        }

        ///--------------------------------------------------------------------------------------------
		/// <summary>
		/// Execute task
		/// </summary>
        public void Execute()
        {
            //_logger.InsertLog(Nop.Core.Domain.Logging.LogLevel.Warning, "Start");
            //_post.post();
           // _likeService.deleteOldData(DateTime.UtcNow);
        }



        public int Order
        {
            //ensure that this task is run first 
            get { return 0; }
        }
    }
}
