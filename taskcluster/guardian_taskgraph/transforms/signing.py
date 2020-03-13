from __future__ import absolute_import, print_function, unicode_literals

import datetime
import os

from taskgraph.transforms.base import TransformSequence
from taskgraph.util.schema import resolve_keyed_by


transforms = TransformSequence()


def _get_dependent_job_name_without_its_kind(dependent_job):
    return dependent_job.label[len(dependent_job.kind) + 1:]


@transforms.add
def build_name_and_attributes(config, tasks):
    for task in tasks:
        dep = task["primary-dependency"]
        task["dependencies"] = {dep.kind: dep.label}
        del task["primary-dependency"]
        task["name"] = _get_dependent_job_name_without_its_kind(dep)

        yield task


@transforms.add
def resolve_keys(config, tasks):
    for task in tasks:
        for key in ("worker-type", "worker.signing-type"):
            resolve_keyed_by(
                task,
                key,
                item_name=task["name"],
                **{
                    'level': config.params["level"],
                }
            )
        yield task
