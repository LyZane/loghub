using System;
using System.Collections.Generic;
using System.Text;

namespace Zane.LogHub.Client
{
    public interface IStorage
    {
        /// <summary>
        /// 将一条日志存储到Storage
        /// </summary>
        /// <param name="log"></param>
        void Enqueue(LogEntity log);

        /// <summary>
        /// 从 Storage 中取出一条日志，该日志会被上传到LogHub.Server，之后会被 Delete。
        /// </summary>
        /// <returns></returns>
        LogEntity DequeueSingle();

        /// <summary>
        /// 从 Storage 中批量取出多条日志，这些日志会被上传到LogHub.Server，之后会被 Delete。
        /// </summary>
        /// <param name="maxCount">允许的最大日志条数。</param>
        /// <returns></returns>
        LogEntity[] DequeueBatch(int maxCount = 100);

        /// <summary>
        /// 删除存储中的 LogEntity
        /// </summary>
        /// <param name="ids"></param>
        void Delete(params string[] ids);
    }
}
