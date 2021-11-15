using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DependencyInjectionContainerLib.Config;
using DependencyInjectionContainerLib;


namespace DependencyInjectionTest
{
    [TestClass]
    public class DependencyTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            DependencyConfig dependencyConfig = new DependencyConfig();
            //dependencyConfig.Register<IDepend, Realization>(ImplementationsTTL.InstancePerDependency);
           
            dependencyConfig.Register<IDepend2, TestClass>(ImplementationsTTL.InstancePerDependency);
            dependencyConfig.Register<IDepend3, TestClass>(ImplementationsTTL.InstancePerDependency);

            dependencyConfig.Register(typeof(IDepend<>), typeof(Realization<>), ImplementationsTTL.InstancePerDependency);

            DependencyProvider provider = new DependencyProvider(dependencyConfig);
            var res = provider.Resolve<IDepend<IDepend3>>();
            string l = "32";  


        }
    }

    interface IDepend<TDep> where TDep:IDepend2
    {
    
    }
    interface IDepend2
    {

    }
    interface IDepend3 : IDepend2 { }

    class Realization<TDepend2> : IDepend<TDepend2> where TDepend2:IDepend2
    {
        private TDepend2 depend;
        public Realization(TDepend2 depend1)
        {
            depend = depend1;
        }

    }

    class TestClass :IDepend2, IDepend3
    {
        //private IDepend depend;
        //public TestClass(IDepend depend1) 
        //{
         //   depend = depend1;
       // }
    }





}
