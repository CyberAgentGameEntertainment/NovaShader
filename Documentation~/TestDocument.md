# Test Documentation
- [Test Documentation](#test-documentation)
  - [Startup](#startup)
    - [Setup Instructions](#setup-instructions)
    - [Verification](#verification)
    - [Running Tests](#running-tests)

## Startup
Nova Shader's visual regression tests use NVIDIA's Flip tool. Mac users need to perform additional setup for permissions as described in the following documentation. (Not required for Windows users)

### Setup Instructions

1. Open Terminal.

2. Navigate to the project root directory:
   ```bash
   cd /path/to/project
   ```

3. Grant execution permissions to the setup script:
   ```bash
   chmod +x setup.sh
   ```

4. Run the setup script:
   ```bash
   ./setup.sh
   ```

5. If you encounter permission errors, run the script with administrator privileges:
   ```bash
   sudo ./setup.sh
   ```

### Verification

When setup completes successfully, you will see the following message:
```
quarantine attribute was successfully removed
```

### Running Tests
Launch the Nova Shader demo and run General/Test Runner, then verify the following:

1. Nova Shader demo application launches successfully
2. Test Runner window is displayed
3. Tests run successfully in Play mode without errors
   1. If some tests fail, the tests might be broken - please contact the person in charge. 