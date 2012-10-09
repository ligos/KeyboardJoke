using System;
using Microsoft.SPOT;
using MurrayGrant.KeyboardJoke.Entities;

namespace MurrayGrant.KeyboardJoke.Services.Fiddlers
{
    public interface IFiddler
    {
        /// <summary>
        /// Called when the KeyboardInput class is constructed.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Called when this fiddler is published.
        /// </summary>
        void OnPublish(Random randomGenerator);

        /// <summary>
        /// Called to apply a fiddle to the output stream.
        /// Note that this may not actually add anything to the output.
        /// </summary>
        void ApplyOnKeyDown(DelayBuffer output, KeyboardKey thisKeyPress, bool isShifted, bool altPressed, bool ctlPressed);

        /// <summary>
        /// Called to apply a fiddle to the output stream.
        /// Note that this may not actually add anything to the output.
        /// </summary>
        void ApplyOnKeyUp(DelayBuffer output, KeyboardKey thisKeyPress, bool isShifted, bool altPressed, bool ctlPressed);

        /// <summary>
        /// Set true when the fiddle has been applied (as some fiddles may need to be interspursed between keystrokes).
        /// </summary>
        bool IsComplete { get; }

        /// <summary>
        /// Called after completion is signalled and the KeyboardInput de-publishes the Fiddler.
        /// </summary>
        void AfterCompletion();
    }
}
