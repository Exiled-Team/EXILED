using System;

namespace Exiled.Loader.Features
{
    /// <summary>
    /// A class that contains the different EXILED loader messages.
    /// </summary>
    public static class LoaderMessages
    {
        /// <summary>
        /// Gets the default loader message.
        /// </summary>
        public static string Default => @"
   ▄████████ ▀████    ▐████▀  ▄█   ▄█          ▄████████ ████████▄
  ███    ███   ███▌   ████▀  ███  ███         ███    ███ ███   ▀███
  ███    █▀     ███  ▐███    ███▌ ███         ███    █▀  ███    ███
 ▄███▄▄▄        ▀███▄███▀    ███▌ ███        ▄███▄▄▄     ███    ███
▀▀███▀▀▀        ████▀██▄     ███▌ ███       ▀▀███▀▀▀     ███    ███
  ███    █▄    ▐███  ▀███    ███  ███         ███    █▄  ███    ███
  ███    ███  ▄███     ███▄  ███  ███▌    ▄   ███    ███ ███   ▄███
  ██████████ ████       ███▄ █▀   █████▄▄██   ██████████ ████████▀
                                  ▀                                 ";

        /// <summary>
        /// Gets the christmas loader message.
        /// </summary>
        public static string Christmas => @"
       __
    .-'  |
   /   <\|
  /     \'         ___   __  __    ___     _       ___     ___
  |_.- o-o        | __|  \ \/ /   |_ _|   | |     | __|   |   \
  / C  -._)\      | _|    >  <     | |    | |__   | _|    | |) |
 /',        |     |___|  /_/\_\   |___|   |____|  |___|   |___/
|   `-,_,__,'   _|""""""""""|_|""""""""""|_|""""""""""|_|""""""""""|_|""""""""""|_|""""""""""|
(,,)====[_]=|   a""`-0-0-'""`-0-0-'""`-0-0-'""`-0-0-'""`-0-0-'""`-0-0-'
  '.   ____/
   | -|-|_
   |____)_)";

        /// <summary>
        /// Gets the loader message according to the actual month.
        /// </summary>
        /// <returns>The correspondent loader message.</returns>
        public static string GetMessage()
        {
            switch (DateTime.Today.Month)
            {
                case 12:
                    return Christmas;
                default:
                    return Default;
            }
        }
    }
}
