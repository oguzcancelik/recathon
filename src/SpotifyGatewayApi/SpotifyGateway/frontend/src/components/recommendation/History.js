import React, {Component} from 'react';
import {connect} from "react-redux";
import {saveUserPlaylists} from "../../store/actions/playlistActions";
import {logout} from "../../store/actions/authActions";
import Loading from "../helpers/Loading";
import HistoryCardContainer from "./HistoryCardContainer";
import CustomRedirect from "../helpers/CustomRedirect";
import * as api from "../../infrastructure/api/api-gateway";

class History extends Component {

    state = {
        loading: true
    };

    componentDidMount() {
        if (!this.props.isLoggedIn || this.props.userPlaylists) {

            this.setState({
                loading: false
            });

            return;
        }

        api.getUserPlaylists()
            .then(res => {
                if (res.status === 200 && !res.data.hasError) {
                    this.props.saveUserPlaylists(res.data.result);
                }

                this.setState({
                    loading: false
                });
            })
            .catch(error => {

                if (error.response.status === 401) {
                    this.props.logout();
                }

                this.setState({
                    loading: false
                });
            });
    }

    render() {
        return this.state.loading
            ? <Loading text="Getting your playlists"/>
            : this.props.userPlaylists
                ? <div><HistoryCardContainer/></div>
                : <CustomRedirect message="User has no recommended playlists. Redirecting to homepage."/>;
    }
}

const mapStateToProps = state => {
    return {
        isLoggedIn: state.auth.isLoggedIn,
        userPlaylists: state.playlist.userPlaylists
    };
};

const mapDispatchToProps = dispatch => {
    return {
        saveUserPlaylists: playlists => dispatch(saveUserPlaylists(playlists)),
        logout: () => dispatch(logout())
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(History);
