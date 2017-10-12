using System;
using System.Collections.Generic;


namespace MTGRLibrary
{
    /// <summary>
    /// Represents a generic Template based Recognizer.
    /// </summary>
    public abstract class Recognizer
    {
        protected string name;
        protected bool verbose;

        public string Name
        {
            get
            {
                return name;
            }
        }

        protected Recognizer(string name)
        {
            this.name = name;
            verbose = false;
        }
        /// <summary>
        /// Convert a generic set of template gestures into the Recognizer's specific gesture representation. 
        /// </summary>
        /// <param name="templates">Set of generic Gesture Templates</param>
        public abstract void SetTemplates(List<Gesture> templates);

        /// <summary>
        /// Classify an input gesture based on a recognizer's specific classification scheme.
        /// </summary>
        /// <param name="inputGesture">The raw unprocessed input gesture</param>
        /// <returns>Classification for the input gesture.</returns>
        public abstract string Classify(Gesture inputGesture);

        /// <summary>
        /// Return a string version of the Recognizer's gesture representation.
        /// </summary>
        /// <param name="inputGesture"></param>
        /// <returns>The processed input gesture as a string.</returns>
        public abstract string gestureToString(Gesture inputGesture);
    }
}
