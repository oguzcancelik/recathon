class ModelConstants:
    Contamination = "contamination"

    DefaultModelParameters = {
        "IsolationForest": {
            "bootstrap": False,
            "contamination": "auto",
            "max_features": 1.0,
            "max_samples": "auto",
            "n_estimators": 100,
            "n_jobs": None,
            "random_state": None,
            "verbose": 0,
            "warm_start": False
        },
        "OneClassSVM": {
            "cache_size": 200,
            "coef0": 0.0,
            "degree": 3,
            "gamma": "scale",
            "kernel": "rbf",
            "max_iter": -1,
            "nu": 0.5,
            "shrinking": True,
            "tol": 0.001,
            "verbose": False
        },
        "LocalOutlierFactor": {
            "algorithm": "auto",
            "contamination": "auto",
            "leaf_size": 30,
            "metric": "minkowski",
            "metric_params": None,
            "n_jobs": None,
            "n_neighbors": 20,
            "novelty": False,
            "p": 2
        }
    }

    SuccessfulModelParameters = {
        "IsolationForest": {
            "bootstrap": False,
            "contamination": 0.7,
            "max_features": 8,
            "max_samples": 100,
            "n_estimators": 100,
            "n_jobs": None,
            "random_state": 42,
            "verbose": 0,
            "warm_start": False
        },
        "OneClassSVM": {
            "cache_size": 200,
            "coef0": 0.0,
            "degree": 3,
            "gamma": 0.00001,
            "kernel": "sigmoid",
            "max_iter": -1,
            "nu": 0.7,
            "shrinking": True,
            "tol": 0.001,
            "verbose": False
        },
        "LocalOutlierFactor": {
            "algorithm": "brute",
            "contamination": 0.5,
            "leaf_size": 30,
            "metric": "sqeuclidean",
            "metric_params": None,
            "n_jobs": None,
            "n_neighbors": 10,
            "novelty": True,
            "p": 2
        }
    }
