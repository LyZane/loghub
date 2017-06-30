using System;
using System.Collections.Generic;
using System.Text;

namespace Zane.LogHub.Client
{
    public abstract class IStorage
    {
        /// <summary>
        /// 将一条日志存储到Storage
        /// </summary>
        /// <param name="log"></param>
        public abstract void Enqueue(LogEntity log);

        /// <summary>
        /// 从 Storage 中取出一条日志，该日志会被上传到LogHub.Server，之后会被 Delete。
        /// </summary>
        /// <returns></returns>
        public abstract LogEntity DequeueSingle();


        /// <summary>
        /// 从 Storage 中批量取出多条日志，这些日志会被上传到LogHub.Server，之后会被 Delete。
        /// </summary>
        /// <param name="maxCount">允许的最大日志条数。</param>
        /// <returns></returns>
        public abstract LogEntity[] DequeueBatch(int maxCount = 1000);

        /// <summary>
        /// 删除 Storage 中的 LogEntity
        /// </summary>
        /// <param name="ids"></param>
        public abstract void Delete(IEnumerable<LogEntity> logs);

        /// <summary>
        /// 删除 Storage 中的 LogEntity
        /// </summary>
        /// <param name="log"></param>
        public abstract void Delete(LogEntity log);

        /// <summary>
        /// 检查当前 Storage 读、写、删 是否正常。
        /// </summary>
        /// <returns>失败时会抛出异常</returns>
        public bool Test()
        {
            var log = new LogEntity("InitTest", "This is a test log, used to detect whether the storage object is ok.");

            try
            {
                // 写
                this.Enqueue(log);  
            }
            catch (Exception ex)
            {
                throw new Exception("Storage 不可用，请检查 Storage 的 写 功能是否正常。");
            }
            try
            {                
                // 读
                this.DequeueSingle();
            }
            catch (Exception ex)
            {
                throw new Exception("Storage 不可用，请检查 Storage 的 读 功能是否正常。");
            }
            try
            {
                // 删
                this.Delete(log);
            }
            catch (Exception ex)
            {
                throw new Exception("Storage 不可用，请检查 Storage 的 删 功能是否正常。");
            }
            return true;
        }
    }
}
