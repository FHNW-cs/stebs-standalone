namespace Stebs.Model.AluCommand
{
    /// <summary>
    /// An operation command to be executed by the ALU.
    /// </summary>
    interface IAluCommand
    {
        /// <summary>
        /// Executes an operation such as add, muliply etc.
        /// </summary>
        /// <returns></returns>
        byte Execute();

        /// <summary>
        /// Sets the flags on the processor which are flagged
        /// by the operation
        /// </summary>
        /// <param name="processor"></param>
        void SetFlags(Processor processor);
        
    }
}
