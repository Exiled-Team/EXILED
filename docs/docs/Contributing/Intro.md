---
sidebar_position: 1
---
# Contributing to EXILED

This is a simple tutorial guiding you to contribute to our framework.

### Forking EXILED
First, create a fork of our [GitHub repository](https://github.com/Exiled-Team/EXILED).

Then, clone it to your computer like so: `git clone https://github.com/your-username/EXILED.git`

Open a terminal in your forked EXILED folder and run ```git checkout dev```. This will switch you to the dev branch, which all pull requests should be submitted to.

### Setting `EXILED_REFERENCES`

If you haven't already, install the `SCP: Secret Laboratory Dedicated Server` through Steam or extract [this zip file](https://exiled.host/build_deps/References.zip) to an easily accessible folder.

#### Windows users
Open the Environment Variables menu by searching for `Environment Variables` in the Start Menu.

Create a new environment variable titled `EXILED_REFERENCES`.

The value should point to `your_steamapps_directory/common/SCP Secret Laboratory Dedicated Server/SCPSL_Data/Managed`, or to the folder where you extracted the zip file mentioned earlier.

#### Linux users
Add `export EXILED_REFERENCES="PATH"` to your `~/.bashrc` or similar file.

PATH should point to `your_steamapps_directory/common/SCP Secret Laboratory Dedicated Server/SCPSL_Data/Managed`, or to the folder where you extracted the zip file mentioned earlier.

---

You should now be able to open the EXILED directory in your favorite IDE.


Once you are done, test your changes thoroughly, and then submit a pull request to the main EXILED repository. Make sure you are targeting the `dev` branch, not `master`!

Happy coding!

## Contributing docs
Documentation is built using [Docosaurus](https://docusaurus.io/docs)