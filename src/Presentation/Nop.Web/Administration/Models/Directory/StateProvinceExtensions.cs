using System;
using Nop.Core.Domain.Directory;

namespace Nop.Services.Directory
{
    public static class StateProvinceExtensions
    {
        /// <summary>
        /// 省市区县面包屑呈现
        /// </summary>
        /// <param name="stateProvince"></param>
        /// <param name="stateProvinceService"></param>
        /// <returns></returns>
        public static string GetStateProvinceBreadCrumb(this StateProvince stateProvince, IStateProvinceService stateProvinceService)
        {
            string result = string.Empty;

            while (stateProvince != null)
            {
                if (String.IsNullOrEmpty(result))
                    result = stateProvince.Name;
                else
                    result = stateProvince.Name + " >> " + result;

                stateProvince = stateProvinceService.GetStateProvinceById(stateProvince.ParentId);

            }
            return result;
        }
    }
}