using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AxBcWrapper.Mapping;
using AxBcWrapper.Session;

using NUnit.Framework;

namespace AxBcWrapperTests
{
    public class QualityOrderMapping : ClassMapping<QualityOrder>
    {
        public QualityOrderMapping()
        {
            TableName("InventQualityOrderTable");
            Field("ItemName", qo => qo.ItemName);
            Field("OrderStatus", qo => qo.OrderStatus);
        }
    }


    public class QualityOrder
    {
        public string ItemName { get; set; }
        public int OrderStatus { get; set; }
    }

    public static class QualityOrderStatus
    {
        public const int Open = 0;
        public const int InProgress = 1;
        public const int Closed = 2;
    }

    [TestFixture]
    public class SessionTrials
    {
        [Test]
        public void DoStuff()
        {   
            var sfc = new AxcSessionConfigurator(@"c:\AXConfigs\ax2012r2_tst.axc");
            var sf = new AxSessionFactory(sfc);
            using (var sess = sf.CreateSession())
            {            
                var orders = sess.Query<QualityOrder>().Where(qo => qo.OrderStatus == QualityOrderStatus.Open).ToList();
            }            
        }
    }
}
