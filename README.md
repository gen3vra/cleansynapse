# CleanSynapse ![No Synapse](https://i.imgur.com/wNP4zOl.png)
<img alt="Maintenance" src="https://img.shields.io/maintenance/yes/2018">

Removes the need to run Synapse in the background to use your custom profiles for Razer peripherals.

# What does this do?
It automatically starts the service required to run your custom profiles for your Razer peripherals. In other words, you can have custom keyboard effects, (custom lighting, waves, whatever) without having to run Synapse in the background. Best part is, it doesn't even run itself in the background. It starts the service and then exits.

Of course, if you want to run Synapse later, you can. It does not interfere with normal operation. If you quit Synapse and your profiles disable, you can simply run the program again.

# How do I acquire this illustrious piece of tech?
Click the "Releases" tab, download the .exe, and run it. If you installed your Razer Synapse to somewhere other than the default, please manually put the .exe in the root of that directory.

# Should I run this at login?
I would. If you ran the program normally and it installed it for you, it should have presented an option to start at login. In that case, it placed a shortcut in *"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup"*. If you installed it manually, you will need to create a shortcut and place it here as well. You will be able to see it in the Task Manager's startup tab after a reboot if you did it correctly.
![help](https://i.imgur.com/Uh30kjR.png)
![help](https://i.imgur.com/8mbnpVt.png)

## Make sure to disable Razer Synapse from running at login as well.
- Open task manager.
- Click on the "startup" tab.
- Click on Razer Synapse.
- If enabled, click "Disable".
![help](https://i.imgur.com/D8yYC1e.png)

# Limitations 
Unfortunately, keybinds that you've set in Synapse will NOT work. I cannot control this without setting up my program to ALSO run in the background exactly like Synapse would anyway. If you want keybinds, might as well just run Synapse. If you're just looking for custom colors, use this.
