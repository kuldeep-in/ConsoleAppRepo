# Dynamics Solution Deployment using CI/CD pipelines

### Build Pipeline (Solution and Portal Data)
- Import Data to $(featurebranch01) Branch
- Job: Dynamics Solution Export
    - Power Platform Tool Installer 
    - Power Platform Export Solution 
    - Power Platform Unpack Solution 
    - Command Like script to Commit Changes
        ```
        echo commit all changes
        git config --global user.email "$(useremail)"
        git config --global user.name "$(username)"

        git fetch --all
        git checkout $(featurebranch01)
        git add --all
        git status

        git commit -m "$(Commit Message)"
        git status

        git -c http.extraheader="AUTHORIZATION: bearer $(System.AccessToken)"
        git push origin $(featurebranch01)
        ```
- Job: Portal Data Export
    - Portal Tool Installer 
    - Export Portal Configuration 
        - Exclude Data : "sitesetting.yml"
        - OutputVariable: portal
    - PS Command line to commit code
        ```
        git config user.email "$(useremail)"
        git config user.name "$(username)"

        git fetch --all
        git checkout $(featurebranch01)

        git add SolutionName\PortalData\Portal-Source
        git add SolutionName\PortalData\Portal-Deployment

        git commit -am "changes"
        git push origin $(featurebranch01)
        ```
    - PS Command line to commit code (removing settings file)
        ```
        git config user.email "$(useremail)"
        git config user.name "$(username)"

        $tables = $(portal.ExcludedTables);
        $lists = $tables.split(",");
        Write-Output $lists
        foreach($l in $lists)
        {
            git rm -r SolutionName\PortalData\Portal-Deployment\custom-portal\$lists
            Write-Output $l removed...
        }

        git fetch --all
        git checkout $(featurebranch01)
        git add .
        git commit -a -m "Remove Sitesettings"

        git push origin $(featurebranch01)
        ```

- Create Pull request from $(featurebranch01) to Main
- Build pipeline on main to generate artifacts
    - Power Platform Tool Installer 
    - Power Platform Pack Solution 
        - Pack the Dynamics365 solution only
    - Copy File
        - SolutionName/PortalData/Portal-Deployment
    - Publish Artifact: drop


- Creation of stored procedures and triggers and UDFs.
    - [Demo]
    - [Labs]
- Partitioning, key choice, horizonal scaling.
- Data Modeling
- Pricing
- Change feeds.
    - Set up change feeds
    - [Demo]

### Day 2 
- Performance Monitoring.
    - Azure monitor