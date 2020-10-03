using System;
using JetBrains.Annotations;
using SimpleJSON;

namespace Utils
{
    public static class JsonNodeExtensions
    {
        public static float RequireFloat(this JSONObject node, string key)
        {
            if (node.HasKey(key))
            {
                var valueNode = node[key];
                if (valueNode.IsNumber)
                    return node[key].AsFloat;

                throw new ArgumentException($"Expected numeric field \"{key}\" in {node}");
            }
            
            throw new ArgumentException($"Missing required field \"{key}\" in {node}");
        }
        
        public static int RequireInt(this JSONObject node, string key)
        {
            if (node.HasKey(key))
            {
                var valueNode = node[key];
                if (valueNode.IsNumber)
                    return node[key].AsInt;

                throw new ArgumentException($"Expected numeric field \"{key}\" in {node}");
            }
            
            throw new ArgumentException($"Missing required field \"{key}\" in {node}");
        }
        
        public static bool RequireBool(this JSONObject node, string key)
        {
            if (node.HasKey(key))
            {
                var valueNode = node[key];
                if (valueNode.IsBoolean)
                    return node[key].AsBool;

                throw new ArgumentException($"Expected boolean field \"{key}\" in {node}");
            }
            
            throw new ArgumentException($"Missing required field \"{key}\" in {node}");
        }
        public static string RequireString(this JSONObject node, string key)
        {
            if (node.HasKey(key))
            {
                var valueNode = node[key];
                if (valueNode.IsString)
                    return node[key].Value;

                throw new ArgumentException($"Expected string field \"{key}\" in {node}");
            }
            
            throw new ArgumentException($"Missing required field \"{key}\" in {node}");
        }
        
        public static JSONObject RequireObject(this JSONObject node, string key)
        {
            if (node.HasKey(key))
            {
                var valueNode = node[key];
                if (valueNode.IsObject)
                    return node[key].AsObject;

                throw new ArgumentException($"Expected object in field \"{key}\" in {node}");
            }
            
            throw new ArgumentException($"Missing required field \"{key}\" in {node}");
        }
        
        public static JSONArray RequireArray(this JSONObject node, string key)
        {
            if (node.HasKey(key))
            {
                var valueNode = node[key];
                if (valueNode.IsArray)
                    return node[key].AsArray;

                throw new ArgumentException($"Expected array in field \"{key}\" in {node}");
            }
            
            throw new ArgumentException($"Missing required field \"{key}\" in {node}");
        }
        
        public static float? OptionalFloat(this JSONObject node, string key, float? defaultValue = null)
        {
            if (node.HasKey(key))
            {
                var valueNode = node[key];
                if (valueNode.IsNumber)
                    return node[key].AsFloat;

                throw new ArgumentException($"Expected numeric field \"{key}\" in {node}");
            }

            return defaultValue;
        }
        
        public static int? OptionalInt(this JSONObject node, string key, int? defaultValue = null)
        {
            if (node.HasKey(key))
            {
                var valueNode = node[key];
                if (valueNode.IsNumber)
                    return node[key].AsInt;

                throw new ArgumentException($"Expected numeric field \"{key}\" in {node}");
            }
            
            return defaultValue;
        }
        
        public static bool? OptionalBool(this JSONObject node, string key, bool? defaultValue = null)
        {
            if (node.HasKey(key))
            {
                var valueNode = node[key];
                if (valueNode.IsBoolean)
                    return node[key].AsBool;

                throw new ArgumentException($"Expected boolean field \"{key}\" in {node}");
            }

            return defaultValue;
        }
        
        [CanBeNull]
        public static string OptionalString(this JSONObject node, string key, string defaultValue = null)
        {
            if (node.HasKey(key))
            {
                var valueNode = node[key];
                if (valueNode.IsString)
                    return node[key].Value;

                throw new ArgumentException($"Expected string field \"{key}\" in {node}");
            }

            return defaultValue;
        }
        
        public static JSONObject OptionalObject(this JSONObject node, string key, JSONObject defaultValue = null)
        {
            if (node.HasKey(key))
            {
                var valueNode = node[key];
                if (valueNode.IsObject)
                    return node[key].AsObject;

                throw new ArgumentException($"Expected object in field \"{key}\" in {node}");
            }

            return defaultValue;
        }
        
        public static JSONArray OptionalArray(this JSONObject node, string key, JSONArray defaultValue = null)
        {
            if (node.HasKey(key))
            {
                var valueNode = node[key];
                if (valueNode.IsArray)
                    return node[key].AsArray;

                throw new ArgumentException($"Expected array in field \"{key}\" in {node}");
            }

            return defaultValue;
        }
    }
}