using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DependencyInjectionContainerLib.Config;
using System.Reflection;

namespace DependencyInjectionContainerLib
{
    public class DependencyProvider
    {
        private DependencyConfig Config;
        private Dictionary<Type, object> DependenciesPool = new Dictionary<Type, object>();


        public DependencyProvider(DependencyConfig dependencyConfig)
        {
            Config = dependencyConfig;
        }


        TDependency Resolve<TDependency>() where TDependency : class
        {
            return  (TDependency)Resolve(typeof(TDependency));
        }

        public object Resolve(Type dependencyType)
        {
            var impl = Config.GetImplementationInfo(dependencyType);
            if (impl.Count == 0)
            {
                throw new ArgumentException("Dependency not registered");
            }
            var usedImpl = impl.First();
            if (DependenciesPool.ContainsKey(dependencyType) && usedImpl.TTL == ImplementationsTTL.Singleton)
            {
                return DependenciesPool[dependencyType];

            }
            else
            {
                var implementation = CreateImplementation(usedImpl.Type);
                if (usedImpl.TTL == ImplementationsTTL.Singleton)
                {
                    DependenciesPool.Add(dependencyType,implementation);
                }

            }


            return null;
        }

        private object CreateImplementation(Type implType)
        {
            foreach (ConstructorInfo constructor in implType.GetConstructors(BindingFlags.Instance | BindingFlags.Public))
            {
                var constructorParams = constructor.GetParameters();
                var @params = new List<object>();
                foreach (ParameterInfo parameterInfo in constructorParams)
                {
                    if (parameterInfo.ParameterType.IsInterface)
                    {
                        @params.Add(this.Resolve(parameterInfo.ParameterType));

                    }
                    else
                    {
                        try
                        {
                            @params.Add(Activator.CreateInstance(parameterInfo.ParameterType));
                        }
                        catch
                        {
                            @params.Add(new object());
                        }
                        
                    }

                    try
                    {
                        return Activator.CreateInstance(implType,@params.ToArray());

                    }
                    catch
                    {
                    }

                }

            }

            throw new Exception("Cannot resolve" + implType.ToString()+" dependency");


            
        }

        

        }
}
