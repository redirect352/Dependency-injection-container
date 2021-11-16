﻿using System;
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
        private Stack<Type> DependecyStack = new Stack<Type>();

        public DependencyProvider(DependencyConfig dependencyConfig)
        {
            Config = dependencyConfig;
        }


        public TDependency Resolve<TDependency>() where TDependency : class
        {
            return  (TDependency)Resolve(typeof(TDependency));
        }
        public object Resolve(Type dependencyType)
        {
            return Resolve(dependencyType, -1);
        }

        public TDependency Resolve<TDependency>(int ind) where TDependency : class
        {
            return (TDependency)Resolve(typeof(TDependency), ind);
        }

        public object Resolve(Type dependencyType, int ind)
        {
            DependecyStack.Push(dependencyType);
            object result = null;
            var impl = Config.GetImplementationInfo(dependencyType, ind);
        

            if (impl.Count == 1)
            {
                var usedImpl = impl.First();
                if (DependenciesPool.ContainsKey(usedImpl.Type) && usedImpl.TTL == ImplementationsTTL.Singleton)
                {
                    result = DependenciesPool[usedImpl.Type];

                }
                else
                {
                    result = CreateImplementation(usedImpl.Type);
                    if (usedImpl.TTL == ImplementationsTTL.Singleton)
                    {
                        DependenciesPool.Add(usedImpl.Type, result);
                    }

                }
            } else if (dependencyType.IsGenericType)
            {
                if (dependencyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    var mainDependency = dependencyType.GetGenericArguments().First();
                    var mainTypeImlementations = Config.GetImplementationInfo(mainDependency,-1);
                    if (mainTypeImlementations.Count>=1)
                    {
                        Type listType = typeof(List<>).MakeGenericType(new Type[] { mainDependency });
                        result =  Activator.CreateInstance(listType);
                        foreach (var usedImpl in mainTypeImlementations)
                        {
                            if (DependenciesPool.ContainsKey(dependencyType) && usedImpl.TTL == ImplementationsTTL.Singleton)
                            {
                                listType.GetMethod("Add").Invoke(result, new object[] { DependenciesPool[usedImpl.Type] });
                            }
                            else
                            {
                                var implementationObject = CreateImplementation(usedImpl.Type);
                                listType.GetMethod("Add").Invoke(result, new object[] { implementationObject });
                                if (usedImpl.TTL == ImplementationsTTL.Singleton)
                                {
                                    DependenciesPool.Add(usedImpl.Type, implementationObject);
                                }

                            }
                        }
                    }


                }
            }
            else if (impl.Count == 0)
            {
                throw new ArgumentException("Dependency not registered");
            }




            DependecyStack.Pop();
            return result;
        
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
                        if (!DependecyStack.Contains(parameterInfo.ParameterType))
                            @params.Add(this.Resolve(parameterInfo.ParameterType));
                        else
                            @params.Add(null);
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
                }

                try
                {
                    return Activator.CreateInstance(implType, @params.ToArray());
                }
                catch{}


            }

            throw new Exception("Cannot resolve" + implType.ToString()+" dependency");


            
        }

        

        }
}
