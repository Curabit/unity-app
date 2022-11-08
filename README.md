# unity-app

### Functionality

- Unity application configured with Pico VR SDK.
- Receives commands from the web interface [app.curabit.in](https://app.curabit.in) to manipulate the virtual environment based on an external users' input.
- Validates headsets using a unique code to distinguish between multiple users and active sessions.
- Transitions between waiting room (waiting for headset validation) and active scene based on appropriate selection through the web interface.
- Actively listens for commands from app.curabit.in to play, pause or stop scene.
- Actively listens for commands from app.curabit.in to manipulate scenes based on external user decision.
