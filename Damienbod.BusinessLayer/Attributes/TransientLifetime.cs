namespace Damienbod.BusinessLayer.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class TransientLifetime : System.Attribute
    {
        public double version;

        public TransientLifetime()
        {
            version = 1.0;
        }
    }
}
 


