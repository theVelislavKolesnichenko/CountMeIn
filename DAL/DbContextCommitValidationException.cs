using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Validation;

namespace DAL
{

    /// <summary>
    /// This exception is thrown when saved/commited data is not correct 
    /// This class cannot be inherited.
    /// </summary>
    public class DbContextCommitValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextCommitValidationException" /> class with no arguments.
        /// </summary>
        public DbContextCommitValidationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextCommitValidationException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public DbContextCommitValidationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextCommitValidationException" /> class with a specified error message and 
        /// a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception. If the <em>innerException</em>
        /// parameter is not a null reference, the current exception is raised in a <strong>catch</strong> block that handles 
        /// the inner exception.</param>
        public DbContextCommitValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

