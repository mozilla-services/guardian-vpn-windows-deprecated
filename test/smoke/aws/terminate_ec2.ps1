Set-DefaultAWSRegion -Region us-east-1
$stack = "guardian-stack-$env:CIRCLE_BUILD_NUM"
# remove the stack
Remove-CFNStack -StackName $stack
# wait stack to delete completely
Wait-CFNStack -StackName $stack -Status DELETE_COMPLETE
