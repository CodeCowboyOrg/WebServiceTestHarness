using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace NumberService
{    

    [ServiceContract(Namespace = WebServiceConstants.CodeCowboyXmlNamespace)]
    public interface INumberService
    {

        [OperationContract]
        string FibonacciSequence(int fibonacciNumber);

        [OperationContract]
        DualOperand Add(DualOperand operands);

        [OperationContract]
        DualOperand Substract(DualOperand operands);

        [OperationContract]
        DualOperand Multiply(DualOperand operands);
    }


    [DataContract]
    public class DualOperand
    {
        int? m_operand1 = null;
        int? m_operand2 = null;
        int? m_answer   = null;

        [DataMember]
        public int? Operand1
        {
            get { return m_operand1; }
            set { m_operand1 = value; }
        }

        [DataMember]
        public int? Operand2
        {
            get { return m_operand2; }
            set { m_operand2 = value; }
        }

        [DataMember]
        public int? Answer
        {
            get { return m_answer; }
            set { m_answer = value; }
        }
    }
}
