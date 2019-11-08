using System;
using System.Linq;
using System.Text;

namespace Booster
{
    public abstract class PathBuilderBase 
    {
        public abstract string GetPathFor(Type type);

        public abstract string GetPathFor<T>();

        protected string GetTypeName(Type type)
        {
            if (!type.IsGenericType) return type.FullName;

            var genericArgs = type.GetGenericArguments().Select(arg => arg.Name);

            return type.Namespace + "-" + type.Name + "-" + string.Join("-", genericArgs);
        }

        protected string CleanUp(string path)
        {
            var sb = new StringBuilder();
            foreach (char c in path)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '-')
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append('-');
                }
            }
            return sb.ToString().ToLower();
        }
    }
}
