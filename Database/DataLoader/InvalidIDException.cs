using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LcaDataLoader {
    [SerializableAttribute]
    public class InvalidIdException : Exception {
        public InvalidIdException() {
        }

        public InvalidIdException(string message)
            : base(message) {
        }

        public InvalidIdException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}
