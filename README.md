# HendryWebApiTemplate

This repository provides a custom .NET Web API template that you can use to quickly set up a new Web API project with a predefined structure and configuration.

## Installation Instructions

Follow the steps below to install and use this template in your own projects.

### Step 1: Install the Template

1. Download or clone this repository to your local machine.
   
2. Open a terminal (Command Prompt, PowerShell, or Terminal) and navigate to the root directory of the template.

3. Run the following command to install the template locally:

   ```bash
   dotnet new install ./HendryWebApiTemplate
   ```
   This command will register the template with your .NET CLI, making it available for use in future projects.

### Step 2: Create a New Project Using the Template

Once the template is installed, you can create a new project using the template by running the following command:

```bash
dotnet new hendrywebapi -n YourProjectName
```

- Replace `hendrywebapi` with the `shortName` specified in the `template.json` file.
- Replace `YourProjectName` with the desired name of your new project.

This will create a new folder named `YourProjectName` with all the necessary files and folder structure based on the template.

### Step 3: Build the New Project

After the project is created, navigate to the project directory:

```bash
cd YourProjectName
```

Then, build the project by running:

```bash
dotnet build
```

This will restore dependencies and build your project, making it ready for development.

## Customizing the Template

If you want to modify the template for your own needs:

1. Open the template directory.
2. Make changes to the template files (e.g., code, configuration, structure).
3. Reinstall the updated template using `dotnet new install ./HendryWebApiTemplate` as described earlier.

## Uninstalling the Template

If you ever need to uninstall the template, run the following command:

```bash
dotnet new uninstall ./HendryWebApiTemplate
```

This will remove the template from your local installation.

---

### Additional Information

For more information on how to create and manage .NET templates, you can refer to the official documentation:

- [Create a Template](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-new-templates)
- [dotnet new command](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-new)

---
