﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Analyzer.Parsing
{
    /// <summary>
    /// Parses most used information from Class Object using Mono.Cecil
    /// </summary>
    public class ParsedClassMonoCecil
    {
        public TypeDefinition TypeObj { get; }   // type object to access class related information
        public string Name { get; }      // Name of Class. (Doesn't include namespace name in it)
        public List<MethodDefinition> Constructors { get; }     // Includes Default Constructors also if created

        /// <summary>
        /// Contains interfaces implemented by the class only the ones specifically mentioned 
        /// Does not include interfaces implemented by the parent class/ implemented interface
        /// This is useful for creation of class relational diagram
        /// </summary>
        public List<InterfaceImplementation> Interfaces { get; }

        public List<MethodDefinition> MethodsList { get; }   // Methods declared only by the class
        public TypeDefinition? ParentClass;      // ParentClass - does not contain classes starting with System/Microsoft
        public List<FieldDefinition> FieldsList;     
        public List<PropertyDefinition> PropertiesList;


        // To store class relationship
        public HashSet<string> CompositionList { get; }
        public HashSet<string> AggregationList { get; }
        public HashSet<string> UsingList { get; }
        public HashSet<string> InheritanceList { get; }

        public ParsedClassMonoCecil(TypeDefinition type)
        {
            TypeObj = type;
            Name = type.Name;

            Constructors = new List<MethodDefinition>();
            MethodsList = new List<MethodDefinition>();
            Interfaces = new List<InterfaceImplementation>();
            FieldsList = new List<FieldDefinition>();
            PropertiesList = new List<PropertyDefinition>();

            // type.Methods will include constructors of the class & will not give methods of parent class
            foreach (MethodDefinition method in type.Methods)
            {
                if (method.IsConstructor)
                {
                    Constructors.Add(method);
                }
                else
                {
                    MethodsList.Add(method);
                }
            }


            // Finding parent class declared in the project - does not contain classes starting with System/Microsoft
            if (type.BaseType.Namespace != null)
            {
                if (!(type.BaseType.Namespace.StartsWith("System") || type.BaseType.Namespace.StartsWith("Microsoft")))
                {
                    ParentClass = TypeObj.BaseType.Resolve();
                }
            }
            else
            {
                ParentClass = TypeObj.BaseType.Resolve();
            }


            // Finding interfaces which are only implemented by the class and declares specifically in the class
            if (type.HasInterfaces)
            {
                Interfaces = type.Interfaces.ToList();

                if (ParentClass?.Interfaces != null)
                {
                    Interfaces = type.Interfaces.Except(ParentClass.Interfaces).ToList();
                }


                HashSet<string> removableInterfaceNames = new();

                foreach(InterfaceImplementation i in Interfaces)
                {
                    foreach (InterfaceImplementation x in i.InterfaceType.Resolve().Interfaces)
                    {
                        removableInterfaceNames.Add(x.InterfaceType.FullName);
                    }
                }

                List<InterfaceImplementation> ifaceList = new();

                foreach (InterfaceImplementation iface in Interfaces)
                {
                    if (!removableInterfaceNames.Contains(iface.InterfaceType.FullName))
                    {
                        ifaceList.Add(iface);
                    }
                }

                Interfaces = ifaceList;
            }

            FieldsList = TypeObj.Fields.ToList();
            PropertiesList = TypeObj.Properties.ToList();


            UsingList = new HashSet<string>();
            CompositionList = new HashSet<string>();
            AggregationList = new HashSet<string>();


            //Inheritance List
            //Adding the parent class (if exist) in the inheritance list
            InheritanceList = new HashSet<string>();
            if (ParentClass != null)
            {
                if (!ParentClass.FullName.StartsWith("System"))
                {
                    InheritanceList.Add("C"+ParentClass.FullName);
                }
            }
            
            //adding all interfaces from which the class inherits, in the inheritance list
            foreach (InterfaceImplementation iface in Interfaces)
            {
                if (!iface.InterfaceType.FullName.StartsWith("System"))
                {
                    InheritanceList.Add("I"+iface.InterfaceType.FullName);
                }
            }
            
            //Composition List
            //Cases: 1. If any parameter of constructor is assigned to a field of the class, then it is composition relationship.
            //2. If any new object is instantiated inside a constructor, and is assigned to any class field, then there exist composition relationship.
            foreach (MethodDefinition ctor in Constructors)
            {
                List<ParameterDefinition> parameterList = ctor.Parameters.ToList();
                if (ctor.HasBody)
                {
                    //iterating over all instructions, to check if any class field (of reference type) is assigned a value (be it from parameter or by instantiating a new object)
                    for (int i = 0; i < ctor.Body.Instructions.Count; i++)
                    {
                        Instruction inst = ctor.Body.Instructions[i];
                        if (inst != null && inst.OpCode == OpCodes.Stfld)
                        {
                            var fieldReference = (FieldReference)inst.Operand;
                            TypeReference fieldType = fieldReference.FieldType;
                            TypeDefinition objType = fieldType.Resolve();

                            // Check if the field type is of reference type (not a value type), not a Generic type, and does not start with "System"
                            if (!fieldType.IsValueType && !objType.IsGenericInstance && !objType.FullName.StartsWith("System"))
                            {
                                if (objType.IsClass && !SetsContainElement("C"+ objType.FullName, InheritanceList))
                                {
                                    CompositionList.Add("C" + objType.FullName);
                                }
                                else if(objType.IsInterface && !SetsContainElement("I" + objType.FullName, InheritanceList))
                                {
                                    CompositionList.Add("I" + objType.FullName);
                                }
                            }
                        }
                    }
                }


                // Handling Case 2 of using relationship, where the parameter of constructor is assigned to a local variable.
                // If between 2 classes composition and using relation exist, giving priority to composition relation.
                foreach (ParameterDefinition parameter in parameterList)
                {
                    TypeDefinition parameterType = parameter.ParameterType.Resolve();
                    string parameterTypeName = parameter.ParameterType.FullName;

                    if (!parameterTypeName.StartsWith("System") && !parameterType.GetType().IsGenericType)
                    {
                        Console.WriteLine(parameterType);

                        Console.WriteLine(parameterType.IsClass);
                        if (parameterType.IsClass && !SetsContainElement("C" + parameter.ParameterType.FullName, InheritanceList, CompositionList))
                        {
                            Console.WriteLine("2: C"+ parameter.ParameterType.FullName);
                            UsingList.Add("C"+parameter.ParameterType.FullName);
                        }
                        else if (parameterType.IsInterface && !SetsContainElement("I" + parameter.ParameterType.FullName, InheritanceList, CompositionList))
                        {
                            Console.WriteLine("2: I" + parameter.ParameterType.FullName);

                            UsingList.Add("I" + parameter.ParameterType.FullName);
                        }
                    }
                }

                //Handling Case 2 of aggregation relationship, where new object is instantiated inside a constructor and is assigned to its local variable.
                //If between 2 classes composition and aggregation relation exists, giving priority to composition relation.
                foreach (MethodDefinition ctr in Constructors)
                {
                    if (ctr.HasBody)
                    {
                        foreach (Instruction inst in ctr.Body.Instructions)
                        {
                            if (inst != null && inst.OpCode == OpCodes.Newobj)
                            {
                                var constructorReference = (MethodReference)inst.Operand;
                                TypeDefinition objectType = constructorReference.DeclaringType.Resolve();

                                // adding to aggregation list, if object is not of generic type and is not in composition list (i.e either the object is assigned to a local variable
                                // or if not, since we have decided on the priority of composition over aggreagation, we can check if the composition list has that particular class object or not).
                                if (!objectType.IsGenericInstance && !objectType.FullName.StartsWith("System"))
                                {
                                    if (objectType.IsClass && !SetsContainElement("C" + objectType.FullName, InheritanceList, CompositionList))
                                    {
                                        AggregationList.Add("C" + objectType.FullName);
                                        UsingList.Remove("C" + objectType.FullName);

                                    }
                                    else if(objectType.IsInterface && !SetsContainElement("I" + objectType.FullName, InheritanceList, CompositionList))
                                    {
                                        AggregationList.Add("I" + objectType.FullName);
                                        UsingList.Remove("I" + objectType.FullName);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Aggregation List
            // Cases: 1. If a new class object is created and/or instantiated inside any method (other than constructor), its aggregation.
            // 2. If a new class object is instantiated inside a constructor, but is not assigned to any class field, its aggregation. 
            // check if new opcode is present in method body and get its type
            foreach (MethodDefinition method in MethodsList)
            {
                if (method.HasBody)
                {
                    foreach (Instruction inst in method.Body.Instructions)
                    {
                        if (inst != null && inst.OpCode == OpCodes.Newobj)
                        {
                            var constructorReference = (MethodReference)inst.Operand;
                            TypeReference objectType = constructorReference.DeclaringType;

                            // adding to aggrgation list, if object is not of generic type
                            if (!objectType.IsGenericInstance && !objectType.FullName.StartsWith("System") && !SetsContainElement("C" + objectType.FullName, InheritanceList, CompositionList))
                            {
                                AggregationList.Add("C" + objectType.FullName);
                            }
                        }
                    }
                }
            }

            // Using Class Relationship 
            // Cases considering: 1. if any method (other than constructors) contain other class as parameter
            // 2.If its a parameter for a constructor, and is not assigned to any field inside the constructor, then it is considered as using relationship. 

            //Get all the methods and its parameters into a dictionary, which can be iterated over to check its types.
            Dictionary<MethodDefinition, List<ParameterDefinition>> dict = GetFunctionParameters();
            foreach (KeyValuePair<MethodDefinition, List<ParameterDefinition>> pair in dict)
            {
                foreach (ParameterDefinition argument in pair.Value)
                {
                    TypeDefinition objType = argument.ParameterType.Resolve();

                    //adding to using list, if the parameter is of class type and is not of generic class (list, dict,etc.)
                    if (objType != TypeObj && objType!= null && !(objType.GetType().IsGenericType))
                    {
                        if (pair.Key.IsConstructor)
                        {
                            continue;
                        }
                        else
                        {
                            //ignoring the classes those start with "System"
                            if (!argument.ParameterType.FullName.StartsWith("System"))
                            {
                                if (objType.IsClass && !SetsContainElement("C" + argument.ParameterType.FullName, InheritanceList, CompositionList, AggregationList))
                                {
                                    Console.WriteLine("1: " + argument.ParameterType.FullName);

                                    UsingList.Add("C" + argument.ParameterType.FullName);
                                }
                                else if (objType.IsInterface && !SetsContainElement("I" + argument.ParameterType.FullName, InheritanceList, CompositionList, AggregationList))
                                {
                                    Console.WriteLine("1: " + argument.ParameterType.FullName);
                                    UsingList.Add("I" + argument.ParameterType.FullName);
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool SetsContainElement<T>(T element, params HashSet<T>[] sets)
        {
            return sets.Any(set => set.Contains(element));
        }

        public Dictionary<MethodDefinition, List<ParameterDefinition>> GetFunctionParameters()
        {
            Dictionary<MethodDefinition, List<ParameterDefinition>> dict = new();

            if (MethodsList != null)
            {
                foreach (MethodDefinition method in MethodsList)
                {
                    dict.Add(method, method.Parameters.ToList());
                }
            }

            return dict;
        }
    }
}
