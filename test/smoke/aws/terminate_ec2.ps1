Set-DefaultAWSRegion -Region us-east-1
# remove the stack
Remove-CFNStack -StackName guardian-stack-$env:CIRCLE_BUILD_NUM
# wait stack to delete completely
Wait-CFNStack -StackName guardian-stack-$env:CIRCLE_BUILD_NUM -Status DELETE_COMPLETE
